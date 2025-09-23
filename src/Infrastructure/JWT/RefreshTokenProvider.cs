using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Domain.Configuration;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UAParser;

namespace Infrastructure.JWT
{
    public class RefreshTokenProvider : IRefreshTokenProvider
    {
        private readonly IConfiguration _configuration;
        private readonly RefreshTokenOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IAppUserRepository _appUserRepository;
        public RefreshTokenProvider(IConfiguration configuration
            ,IOptions<RefreshTokenOptions> options
            ,IHttpContextAccessor httpContextAccessor
            ,IUserRepository userRepository
            ,IAppUserRepository appUserRepository)
        {
            _configuration = configuration;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _appUserRepository = appUserRepository;
        }
        public async Task<RefreshToken> GenerateRefreshToken(Guid userId)
        {
            var refreshTokenValidityDays = _options.ExpireDays;

            var uaString = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";

            var parser = Parser.GetDefault();

            ClientInfo client = parser.Parse(uaString);

            var device_info = $"{client.UA.Family} on {client.OS.Family}";

            var appUser = await _appUserRepository.GetAppUserAsync(userId);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(refreshTokenValidityDays),
                UserId = userId,
                AppUser = appUser,
                DeviceInfo = device_info
            };
            return refreshToken;
        }
        public string RefreshTokenHasher(string Token)
        {
            using SHA256 sha256 = SHA256.Create();

            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Token));

            return Convert.ToBase64String(bytes);
        }
    }
}
