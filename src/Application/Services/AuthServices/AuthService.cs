using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Domain.Configuration;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Application.Interfaces.Services;
using Application.DTO.AuthDTO;
namespace Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasher _Hasher;
        private readonly ITokenService _tokenService;
        private readonly IOptions<JwtOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAuthRepository _authRepository;
        public DateTime kazTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty"));
        public DateTime utcNow = DateTime.UtcNow;
        public AuthService(IUserRepository userRepository
            , IHasher Hasher
            , IOptions<JwtOptions> options
            , IHttpContextAccessor httpContextAccessor
            , ITokenService tokenService
            , IAppUserRepository appUserRepository
            , IAuthRepository authRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _Hasher = Hasher;
            _options = options;
            _tokenService = tokenService;
            _appUserRepository = appUserRepository;
            _authRepository = authRepository;
        }
        public async Task<LoginStatusDTO> LoginPin(string email, string pinCode)
        {
            var appUser = await _appUserRepository.GetAppUserByEmail(email);
            if (appUser == null)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "There is no such app user, please register"
                };
            }
            if (appUser.BlockedUntil != null && appUser.BlockedUntil > utcNow)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = $"User is blocked until {TimeZoneInfo.ConvertTimeFromUtc(appUser.BlockedUntil.Value,
                                TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty"))}"
                };
            }
            if (appUser.CountOfLoginAttempts > 3)
            {
                var utcNow = DateTime.UtcNow;
                appUser.BlockedUntil = utcNow.AddHours(3);
                await _userRepository.SaveChangesAsync();
            }
            var result_verification = _Hasher.Verify(pinCode, appUser.HashedPinCode!);
            if (result_verification == false)
            {
                appUser.CountOfLoginAttempts++;
                await _userRepository.SaveChangesAsync();
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "Incorrect pin code"
                };
            }
            appUser.BlockedUntil = null;
            appUser.CountOfLoginAttempts = 0;
            appUser.BlockedUntilPassword = null;
            appUser.CountOfLoginViaPasswordAttempts = 0;
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
            await _authRepository.SaveRefreshToken(hashed_one);
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
        public async Task<LoginStatusDTO> LoginPassword(string email, string password)
        {
            var appUser = await _appUserRepository.GetAppUserByEmail(email);
            if (appUser == null)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "There is no such app user, please register"
                };
            }
            if (appUser.BlockedUntilPassword != null && appUser.BlockedUntilPassword > utcNow)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = $"User is blocked until {TimeZoneInfo.ConvertTimeFromUtc(appUser.BlockedUntilPassword.Value,
                                TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty"))}"
                };
            }
            if (appUser.CountOfLoginViaPasswordAttempts > 3)
            {
                var utcNow = DateTime.UtcNow;
                appUser.BlockedUntilPassword = utcNow.AddHours(3);
                await _userRepository.SaveChangesAsync();
            }
            var result_verification = _Hasher.Verify(password, appUser.PasswordHash!);
            if (result_verification == false)
            {
                appUser.CountOfLoginViaPasswordAttempts++;
                await _userRepository.SaveChangesAsync();
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "Incorrect password"
                };
            }
            appUser.BlockedUntil = null;
            appUser.CountOfLoginAttempts = 0;
            appUser.BlockedUntilPassword = null;
            appUser.CountOfLoginViaPasswordAttempts = 0;
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
            await _authRepository.SaveRefreshToken(hashed_one);
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
            if (!_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "No refresh token"
                };
            }
            var hashedRefreshToken = _tokenService.HashRefreshToken(refreshToken);
            var storedRefreshToken = await _authRepository.FindRefreshToken(hashedRefreshToken);
            if (storedRefreshToken == null)
            {
                return new LoginStatusDTO
                {
                    VerificationStatus = VerificationStatus.Rejected.ToString(),
                    Message = "Invalid refresh token"
                };
            }
            var appUser = storedRefreshToken.AppUser;
            if (storedRefreshToken.Expires < utcNow)
            {
                storedRefreshToken.RevokedAt = utcNow;
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
