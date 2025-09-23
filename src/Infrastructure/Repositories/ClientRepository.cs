using Application.Interfaces.Repositories;
using BankSystem;
using Domain.Models;
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
        public async Task<int> DeleteAsync(string IIN)
        {
            int affected = await _context.Clients
            .Where(c => c.IIN == IIN)
            .ExecuteDeleteAsync();

            return affected;
        }
    }
}
