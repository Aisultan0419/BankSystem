using System.Reflection.Metadata.Ecma335;
using Application.Interfaces.Repositories;
using BankSystem;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsByIINAsync(string iin)
        {
            bool exists = await _context.Clients.AnyAsync(client => client.IIN == iin);
            return exists;
        }
        public async Task<Client?> FindByIINAsync(string iin)
        {
            Client? client = await _context.Clients.FirstOrDefaultAsync(client => client.IIN == iin);
            return client;
        }
        public async Task SaveDataClientAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Unsuccessful attempt to save data", ex);
            }
        }
        public async Task SaveDataAppUserAsync(AppUser appUser)
        {
            await _context.AppUsers.AddAsync(appUser);
        }
        public async Task<int> DeleteAsync(string IIN)
        {
            int affected = await _context.Clients
            .Where(c => c.IIN == IIN)
            .ExecuteDeleteAsync();

            return affected;
        }
        public async Task<AppUser> GetAppUserAsync(Guid Id)
        {
            var result = await _context.AppUsers.AsNoTracking().FirstOrDefaultAsync(appUser => appUser.Id == Id);
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
