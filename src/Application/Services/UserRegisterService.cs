using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Application.Services
{
    public class UserRegisterService : IUserRegisterService
    {
       
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtprovider;
        public UserRegisterService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtprovider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtprovider = jwtprovider;
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
        public async Task<LoginStatusDTO> Login(string email, string password)
        {
            var appUser = await _userRepository.GetAppUserByEmail(email);

            if (appUser == null)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "There is no such app user, please register"
                };
            }

            var result_verification = _passwordHasher.Verify(password, appUser.PasswordHash!);
            if (result_verification == false)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "Incorrect password"
                };
            }
            var token = _jwtprovider.GenerateToken(appUser);
            return new LoginStatusDTO
            {
                VerificationStatus = VerificationStatus.Verified.ToString(),
                Message = "User is logined successfully",
                Token = token
            };
        }
    }
}
