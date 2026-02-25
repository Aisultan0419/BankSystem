using Application.Interfaces.Auth;
using Application.Interfaces.Services.Auths;
using Domain.Models;

namespace Application.Services.AuthServices
{
    public class TokenService : ITokenService
    {
        private readonly IJwtProvider _jwtprovider;
        private readonly IRefreshTokenProvider _refreshTokenProvider;

        public TokenService(
            IJwtProvider jwtprovider,
            IRefreshTokenProvider refreshTokenProvider)
        {
            _jwtprovider = jwtprovider;
            _refreshTokenProvider = refreshTokenProvider;
        }

        public string GenerateJwt(AppUser appUser) => _jwtprovider.GenerateToken(appUser);

        public Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
            => _refreshTokenProvider.GenerateRefreshToken(userId);

        public string HashRefreshToken(string token) => _refreshTokenProvider.RefreshTokenHasher(token);
    }
}
