using Application.DTO.AppUserDTO;
using Application.DTO.ClientDTO;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.AppUsers;
using Domain.Enums;
using Domain.Models;
namespace Application.Services.AppUserServices
{
    public class AppUserService : IAppUserService
    {
       
        private readonly IUserRepository _userRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IHasher _Hasher;
        private readonly IClientRepository _clientRepository;
        public AppUserService(IUserRepository userRepository
            ,IHasher Hasher
            ,IAppUserRepository appUserRepository
            ,IClientRepository clientRepository)
        {
            _userRepository = userRepository;
            _Hasher = Hasher;
            _appUserRepository = appUserRepository;
            _clientRepository = clientRepository;
        }
        public async Task<RegistrationStatusDTO> RegisterAppUser(AppUserCreateDTO AppUserDTO)
        {
            var client = await _clientRepository.FindByIINAsync(AppUserDTO.IIN);
            if (client == null)
            {
                return new RegistrationStatusDTO
                {
                    KycStatus = KycStatus.NotStarted.ToString(),
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "There is no such client to connect"
                };
            }
            var appUser = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = AppUserDTO.Email,
                PasswordHash = _Hasher.Generate(AppUserDTO.PasswordHash!),
                HashedPinCode = _Hasher.Generate(AppUserDTO.PinCode!),
                VerificationStatus = VerificationStatus.Pending,
                Client = client,
                IsActive = false
            };
            var appuserexists = await _appUserRepository.ExistsByEmailAsync(appUser.Email);
            if (appuserexists)
            {
                return new RegistrationStatusDTO
                {
                    KycStatus = KycStatus.NotStarted.ToString(),
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "There is already such app user in the system"
                };
            }
            appUser.VerificationStatus = VerificationStatus.Verified;
            appUser.IsActive = true;

            await _appUserRepository.SaveDataAppUserAsync(appUser);

            await _userRepository.SaveChangesAsync();
            
            return new RegistrationStatusDTO
            {
                KycStatus = KycStatus.Verified.ToString(),
                VerificationStatus = VerificationStatus.Verified.ToString(),
                Message = "The user successfully has been saved"
            };
        }
    }
}
