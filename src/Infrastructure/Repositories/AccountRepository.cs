using Application.DTO;
using Application.Interfaces.Repositories;
using BankSystem;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddEncyptedPan(Pan pan)
        {
            await _context.Pans.AddAsync(pan);
        }
        public async Task AddCard(Card card)
        {
            await _context.Cards.AddAsync(card);
        }
        public async Task AddAccount(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }
        public async Task<IEnumerable<GetCardDTO>> GetAllCards(Guid clientId)
        {
            var list = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .SelectMany(a => a.Cards.Select(card => new GetCardDTO
            {
                PanMasked = card.PanMasked,
                Balance = a.Balance,
                Status = card.Status
            }))
            .ToListAsync();
            return list;
        }
        public async Task<Card> GetRequisitesDTOAsync(Guid clientId, string last_numbers)
        {
            var result = await _context.Clients
                .AsNoTracking()
                .Include(ac => ac.Accounts)
                    .ThenInclude(ca => ca.Cards)
                        .ThenInclude(ka => ka.Pan)
                .Where(cl => cl.Id == clientId)
                .SelectMany(a => a.Accounts)
                .SelectMany(c => c.Cards)
                .FirstOrDefaultAsync(cr => cr.PanMasked!.EndsWith(last_numbers));

            return result ?? throw new Exception("Card is null");
        }
    }
}
