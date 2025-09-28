﻿using Application.Interfaces.Repositories;
using BankSystem;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _context;

        public AppUserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task SaveDataAppUserAsync(AppUser appUser)
        {
            await _context.AppUsers.AddAsync(appUser);
        }
        public async Task<AppUser> GetAppUserAsync(Guid Id)
        {
            var result = await _context.AppUsers.AsNoTracking()
                .Include(u => u.Client)
                .FirstOrDefaultAsync(appUser => appUser.Id == Id);
            return result!;
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            bool exists = await _context.AppUsers.AnyAsync(user => user.Email == email);
            return exists;
        }
        public async Task<AppUser?> GetAppUserByEmail(string email)
        {
            AppUser? appUser = await _context.AppUsers.FirstOrDefaultAsync(user => user.Email == email);
            return appUser;
        }
    }
}
