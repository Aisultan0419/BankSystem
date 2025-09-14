using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Auth;
using Domain.Models;

namespace Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateJwt(AppUser appUser);
        Task<RefreshToken> CreateRefreshTokenAsync(Guid userId);
        string HashRefreshToken(string token);
    }
}
