using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure;
using Infrastructure.DbContext;
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
        public async Task<string?> GetOrderNumber()
        {
            var number = await _context.Accounts.CountAsync();
            return number.ToString("D6");
        }
    }
}
