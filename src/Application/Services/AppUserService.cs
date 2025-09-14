using Application.DTO;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Models;
namespace Application.Services
{
    public class AppUserService : IAppUserService
    {
       
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public AppUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<RegistrationStatusDTO> RegisterAppUser(AppUserCreateDTO AppUserDTO)
        {
            var client = await _userRepository.FindByIINAsync(AppUserDTO.IIN);
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
                PasswordHash = _passwordHasher.Generate(AppUserDTO.PasswordHash!),
                VerificationStatus = VerificationStatus.Pending,
                Client = client,
                IsActive = false
            };
            var appuserexists = await _userRepository.ExistsByEmailAsync(appUser.Email);
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

            await _userRepository.SaveDataAppUserAsync(appUser);

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
