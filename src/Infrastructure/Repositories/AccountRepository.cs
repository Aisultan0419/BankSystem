using Application.DTO.CardDTO;
using Application.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Accounts;
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
        public async Task AddAccount(CurrentAccount account)
        {
            await _context.CurrentAccounts.AddAsync(account);
        }
        public async Task<IEnumerable<GetCardDTO>> GetAllCards(Guid clientId)
        {
            var list = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .SelectMany(a => a.Cards.Select(card => new GetCardDTO(
                card.PanMasked,
                card.Status.ToString(),
                a.Balance,
                "KZT"
                )
            ))
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
                    .ThenInclude(ca => ca!.Cards)
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
        public async Task AddSavingAccount(SavingAccount savingAccount)
        {
            await _context.SavingsAccounts.AddAsync(savingAccount);
        }
        public async Task<bool> IsDayForAccrualInterestDeposit(Account account)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var alreadyAccruedThisMonth = await _context.InterestAccrualHistory
                .AsNoTracking()
                .AnyAsync(h => h.AccountId == account.Id
                            && h.AccrualDate.Year == today.Year
                            && h.AccrualDate.Month == today.Month);
            return today.Day == 1 && !alreadyAccruedThisMonth;
        }

        public async Task<bool> CheckForIdempotencyOfAccrualInterest(Account account)
        {
            var firstRecord = await _context.InterestAccrualHistory
                .AsNoTracking()
                .Where(a => a.AccountId == account.Id && a.AccrualDate == DateOnly.FromDateTime(DateTime.Today))
                .FirstOrDefaultAsync();
            if (firstRecord == default)
            {
                return false;
            }
            return true;
        }
    }
}
