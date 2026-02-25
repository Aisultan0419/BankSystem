using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> FindRefreshToken(string refreshToken)
        {
            var result = await _context.RefreshTokens.AsNoTracking().Include(rt => rt.AppUser).FirstOrDefaultAsync(token => token.Token == refreshToken);
            return result;
        }
    }
}
