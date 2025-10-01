using Application.DTO.CardDTO;
using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.DbContext;
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
                Status = card.Status.ToString(),
            }))
            .ToListAsync();
            return list;
        }
        public async Task<Account> GetAccountById(Guid accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            return account ?? throw new ArgumentNullException();
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
        public async Task<Account> GetAccountByIban(string iban)
        {
            var account = await _context.Accounts.Include(c => c.Client).FirstOrDefaultAsync(a => a.Iban == iban);
            return account ?? throw new ArgumentNullException();
        }
    }
}
