using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Auth;
using Domain.Configuration;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.JWT
{
    public class RefreshTokenProvider : IRefreshTokenProvider
    {
        private readonly IConfiguration _configuration;
        private readonly RefreshTokenOptions _options;
        public RefreshTokenProvider(IConfiguration configuration, IOptions<RefreshTokenOptions> options)
        {
            _configuration = configuration;
            _options = options.Value;
        }
        public RefreshToken GenerateRefreshToken(Guid userId)
        {
            var refreshTokenValidityDays = _options.ExpireDays;
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(refreshTokenValidityDays),
                UserId = userId
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
