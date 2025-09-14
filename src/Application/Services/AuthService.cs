using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Domain.Configuration;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Application.Interfaces.Services;
namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IOptions<JwtOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IUserRepository userRepository
            , IPasswordHasher passwordHasher
            , IOptions<JwtOptions> options
            , IHttpContextAccessor httpContextAccessor
            , ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _options = options;
            _tokenService = tokenService;
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
            var token = _tokenService.GenerateJwt(appUser);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(appUser.Id);
            var hashed_one = new RefreshToken
            {
                Id = refreshToken.Id,
                Token = _tokenService.HashRefreshToken(refreshToken.Token),
                Expires = refreshToken.Expires,
                AppUser = appUser,
                UserId = appUser.Id
            };
            await _userRepository.SaveRefreshToken(hashed_one);
            await _userRepository.SaveChangesAsync();
            return new LoginStatusDTO
            {
                VerificationStatus = VerificationStatus.Verified.ToString(),
                Message = "User is logined successfully",
                Token = token,
                RefreshToken = refreshToken.Token,
                ExpiresIn = _options.Value.ExpireMinutes,
                UserId = appUser.Id,
                DeviceInfo = refreshToken.DeviceInfo
            };
        }
        public async Task<LoginStatusDTO> RefreshToken()
        {
            if (!(_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)))
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "No refresh token"
                };
            }
            var hashedRefreshToken = _tokenService.HashRefreshToken(refreshToken);
            var storedRefreshToken = await _userRepository.FindRefreshToken(hashedRefreshToken);
            if (storedRefreshToken == null)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "Invalid refresh token"
                };
            }
            var appUser = storedRefreshToken.AppUser;
            if (storedRefreshToken.Expires < DateTime.UtcNow)
            {
                storedRefreshToken.RevokedAt = DateTime.UtcNow;
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "Expired refresh token"
                };
            }
            var token = _tokenService.GenerateJwt(storedRefreshToken.AppUser);
            return new LoginStatusDTO
            {
                VerificationStatus = VerificationStatus.Verified.ToString(),
                Message = "User is relogined successfully",
                Token = token,
                RefreshToken = storedRefreshToken.Token,
                ExpiresIn = _options.Value.ExpireMinutes,
                UserId = appUser!.Id,
                DeviceInfo = storedRefreshToken.DeviceInfo
            };
        }
    }
}
