using Application.DTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IUserRepository _userRepository;
        public ClientService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            var exists = await _userRepository.ExistsByIINAsync(client.IIN!);

            if (exists) return new RegistrationStatusDTO
            {
                KycStatus = KycStatus.Rejected.ToString(),
                Message = "Client data is already in the system!"
            };
            client.KycStatus = KycStatus.Verified;
            await _userRepository.SaveDataClientAsync(client);

            await _userRepository.SaveChangesAsync();

            return new RegistrationStatusDTO()
            {
                KycStatus = KycStatus.Verified.ToString(),
                Message = "Client successfully has been saved"
            };
        }
        public async Task<bool> DeleteClient(string IIN)
        {
            var affected = await _userRepository.DeleteAsync(IIN);
            return affected > 0;
        }
    }
}
