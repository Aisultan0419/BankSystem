using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Auths;
using Domain.Configuration;
using Domain.Models;
using Microsoft.Extensions.Options;

namespace Application.Services.AuthServices
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtprovider;
        private readonly IRefreshTokenProvider _refreshTokenProvider;
        private readonly IOptions<JwtOptions> _options;
        public TokenService(IUserRepository userRepository
            , IJwtProvider jwtprovider
            , IRefreshTokenProvider refreshTokenProvider
            , IOptions<JwtOptions> options)
        {
            _userRepository = userRepository;
            _jwtprovider = jwtprovider;
            _refreshTokenProvider = refreshTokenProvider;
            _options = options;
        }
        public string GenerateJwt(AppUser appUser) => _jwtprovider.GenerateToken(appUser);
        public Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
            => _refreshTokenProvider.GenerateRefreshToken(userId);
        public string HashRefreshToken(string token) => _refreshTokenProvider.RefreshTokenHasher(token);
    }
}
