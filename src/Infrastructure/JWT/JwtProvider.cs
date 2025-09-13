using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Interfaces.Auth;
using Domain.Configuration;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Infrastructure.JWT
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GenerateRefreshToken()
        {
            const int size = 32;

            byte[] bytes = new byte[size];

            using var rng = RandomNumberGenerator.Create(); 

            rng.GetBytes(bytes);

            using SHA256 sha256 = SHA256.Create();
            
            var key = sha256.ComputeHash(bytes);
            
            return Convert.ToBase64String(bytes);
        }
        public string GenerateToken(AppUser appUser)
        {
            Claim[] claims = new Claim[] 
            { 
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var signingCredentials = new SigningCredentials
                (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    audience: _options.Audience,
                    issuer: _options.Issuer,
                    claims: claims,
                    signingCredentials: signingCredentials,
                    expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes)
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
