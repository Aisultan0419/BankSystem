using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsByIinAsync(string iin)
        {
            bool exists = await _context.Clients.AnyAsync(client => client.IIN == iin);
            return exists;
        }
        public async Task<Client?> FindByIinAsync(string iin)
        {
            Client? client = await _context.Clients.FirstOrDefaultAsync(client => client.IIN == iin);
            return client;
        }

        public async Task<int> DeleteAsync(string IIN)
        {
            int affected = await _context.Clients
            .Where(c => c.IIN == IIN)
            .ExecuteDeleteAsync();

            return affected;
        }
    }
}
