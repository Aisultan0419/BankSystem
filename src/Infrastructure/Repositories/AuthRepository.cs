using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using BankSystem;
using Domain.Models;
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
        public async Task SaveRefreshToken(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }
        public async Task<RefreshToken?> FindRefreshToken(string refreshToken)
        {
            var result = await _context.RefreshTokens.AsNoTracking().Include(rt => rt.AppUser).FirstOrDefaultAsync(token => token.Token == refreshToken);
            return result;
        }
    }
}
