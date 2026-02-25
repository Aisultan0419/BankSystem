using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Domain.Configuration;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Application.DTO.AuthDTO;
using Application.Interfaces.Services.Auths;
using Domain.Models.Accounts;
namespace Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly IOptions<JwtOptions> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IClock _clock;
        private static readonly TimeZoneInfo kazTime = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");

        public AuthService(
            IUnitOfWork unitOfWork,
            IHasher hasher,
            IOptions<JwtOptions> options,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            IAppUserRepository appUserRepository,
            IAuthRepository authRepository,
            IClock clock)
        {
            _unitOfWork = unitOfWork;
            _hasher = hasher;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _appUserRepository = appUserRepository;
            _authRepository = authRepository;
            _clock = clock;
        }

        public async Task<LoginStatusDTO> LoginProcess(string email, string input)
        {
            bool isItPinCode = input.Length == 4;
            var appUser = await _appUserRepository.GetAppUserByEmail(email);
            if (appUser == null)
            {
                return Reject("There is no such user in the system");
            }
            var isItBlocked = CheckForBlockedAppUser(isItPinCode, appUser);
            if (isItBlocked != null)
            {
                return isItBlocked;
            }
            var verificationResult = await VerifyAppUser(isItPinCode, input, appUser);
            if (verificationResult != null)
            {
                return verificationResult;
            }

            UserAttemptsReset(appUser);
            var token = _tokenService.GenerateJwt(appUser);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(appUser.Id);
            var hashed_one = GenerateRefreshBody(appUser, refreshToken);
            await _unitOfWork.AddItem(hashed_one);
            await _unitOfWork.SaveChangesAsync();
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

        private LoginStatusDTO? CheckForBlockedAppUser(bool isItPinCode, AppUser appUser)
        {
            if (isItPinCode)
            {
                if (appUser.BlockedUntil != null)
                {
                    var blockedUntil = DateTime.SpecifyKind(appUser.BlockedUntil.Value, DateTimeKind.Utc);
                    if (blockedUntil > _clock.UtcNowOffset.UtcDateTime)
                    {
                        return Reject($"User is blocked until {TimeZoneInfo.ConvertTimeFromUtc(blockedUntil, kazTime)}");
                    }
                }
            }
            else
            {
                if (appUser.BlockedUntilPassword != null && appUser.BlockedUntilPassword > _clock.UtcNowOffset)
                {
                    return Reject($"User is blocked until {TimeZoneInfo.ConvertTimeFromUtc(appUser.BlockedUntilPassword.Value, kazTime)}");
                }
            }
            return null;
        }

        private async Task<LoginStatusDTO?> VerifyAppUser(bool isItPinCode, string input, AppUser appUser)
        {
            var result_verification = isItPinCode ? _hasher.Verify(input, appUser.HashedPinCode!) : _hasher.Verify(input, appUser.PasswordHash!);
            if (!result_verification && isItPinCode is true)
            {
                appUser.CountOfLoginAttempts++;
                if (appUser.CountOfLoginAttempts > 3)
                {
                    var blockUntilUtc = _clock.UtcNowOffset.AddHours(3).UtcDateTime;
                    appUser.BlockedUntil = blockUntilUtc;
                    await _unitOfWork.SaveChangesAsync();

                    var localBlocked = TimeZoneInfo.ConvertTimeFromUtc(blockUntilUtc, kazTime);
                    return Reject($"User is blocked until {localBlocked}");
                }
                await _unitOfWork.SaveChangesAsync();
                return Reject("Incorrect pin code");
            }
            if (!result_verification && isItPinCode is false)
            {
                appUser.CountOfLoginViaPasswordAttempts++;
                if (appUser.CountOfLoginViaPasswordAttempts > 3)
                {
                    var blockUntilUtc = _clock.UtcNowOffset.AddHours(3).UtcDateTime;
                    appUser.BlockedUntilPassword = blockUntilUtc;
                    await _unitOfWork.SaveChangesAsync();

                    var localBlocked = TimeZoneInfo.ConvertTimeFromUtc(blockUntilUtc, kazTime);
                    return Reject($"User is blocked until {localBlocked}");
                }
                await _unitOfWork.SaveChangesAsync();
                return Reject("Incorrect password");
            }
            return null;
        }

        private LoginStatusDTO Reject(string message)
        {
            return new LoginStatusDTO
            {
                VerificationStatus = VerificationStatus.Rejected.ToString(),
                Message = message
            };
        }

        private void UserAttemptsReset(AppUser appUser)
        {
            appUser.CountOfLoginAttempts = 0;
            appUser.BlockedUntil = null;
            appUser.CountOfLoginViaPasswordAttempts = 0;
            appUser.BlockedUntilPassword = null;
        }   

        private RefreshToken GenerateRefreshBody(AppUser appUser, RefreshToken refreshToken)
        {
            return new RefreshToken
            {
                Id = refreshToken.Id,
                Token = _tokenService.HashRefreshToken(refreshToken.Token),
                Expires = refreshToken.Expires,
                AppUser = appUser,
                UserId = appUser.Id
            };
        }

        public async Task<LoginStatusDTO> RefreshToken()
        {
            var utcNow = DateTime.UtcNow;
            if (!_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return Reject("Refresh token is missing");
            }
            var hashedRefreshToken = _tokenService.HashRefreshToken(refreshToken);
            var storedRefreshToken = await _authRepository.FindRefreshToken(hashedRefreshToken);
            if (storedRefreshToken == null)
            {
                return Reject("Invalid refresh token");
            }
            var appUser = storedRefreshToken.AppUser;
            if (storedRefreshToken.Expires < utcNow)
            {
                storedRefreshToken.RevokedAt = utcNow;
                return Reject("Refresh token has expired");
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
