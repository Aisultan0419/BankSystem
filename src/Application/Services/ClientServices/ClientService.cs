using Application.DTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Application.Services.ClientServices
{
    public class ClientService : IClientService
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAccountService _accountService;
        public ClientService(IUserRepository userRepository
            ,IClientRepository clientRepository
            ,IAccountService accountService)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _accountService = accountService;
        }
        public async Task<RegistrationStatusDTO> Register(ClientCreateDTO ClientDTO)
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                IIN = ClientDTO.IIN,
                KycStatus = KycStatus.Pending,
                FullName = ClientDTO.FullName,
                IsDeleted = false,
                PhoneNumber = ClientDTO.PhoneNumber
            };
            
            var exists = await _clientRepository.ExistsByIINAsync(client.IIN!);

            if (exists) return new RegistrationStatusDTO
            {
                KycStatus = KycStatus.Rejected.ToString(),
                Message = "Client data is already in the system!"
            };
            client.KycStatus = KycStatus.Verified;
            await _clientRepository.SaveDataClientAsync(client);

            await _userRepository.SaveChangesAsync();
            await _accountService.CreateAccount(client);

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
