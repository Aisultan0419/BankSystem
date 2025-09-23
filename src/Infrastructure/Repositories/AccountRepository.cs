using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using BankSystem;
using Domain.Models;

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
    }
}
