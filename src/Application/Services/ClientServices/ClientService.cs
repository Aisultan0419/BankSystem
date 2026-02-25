using Application.DTO.ClientDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.Clients;
using Domain.Enums;
using Domain.Models;

namespace Application.Services.ClientServices
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientRepository _clientRepository;
        private readonly IAccountService _accountService;
        private readonly IAccountRepository _accountRepository;

        public ClientService(
            IUnitOfWork unitOfWork,
            IClientRepository clientRepository,
            IAccountService accountService,
            IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _clientRepository = clientRepository;
            _accountService = accountService;
            _accountRepository = accountRepository;
        }

        public async Task<RegistrationStatusDTO> Register(ClientCreateDTO ClientDTO)
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                IIN = ClientDTO.Iin,
                KycStatus = KycStatus.Pending,
                FullName = ClientDTO.FullName,
                IsDeleted = false,
                PhoneNumber = ClientDTO.PhoneNumber
            };
            
            var exists = await _clientRepository.ExistsByIinAsync(client.IIN!);

            if (exists) return new RegistrationStatusDTO
            {
                KycStatus = KycStatus.Rejected.ToString(),
                Message = "Client data is already in the system!"
            };
            client.KycStatus = KycStatus.Verified;
            await _unitOfWork.AddItem(client);

            var account = await _accountService.CreateAccount(client);
            await _unitOfWork.AddItem(account);
            await _unitOfWork.SaveChangesAsync();
            return new RegistrationStatusDTO()
            {
                KycStatus = KycStatus.Verified.ToString(),
                Message = "Client successfully has been saved"
            };
        }

        public async Task<bool> DeleteClient(string IIN)
        {
            var affected = await _clientRepository.DeleteAsync(IIN);
            return affected > 0;
        }
    }
}
