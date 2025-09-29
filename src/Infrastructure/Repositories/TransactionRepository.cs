using Application.Interfaces.Repositories;
using BankSystem;
using Microsoft.EntityFrameworkCore.Storage;
using Domain.Models;

namespace Infrastructure.Repositories
{
        public class TransactionRepository : ITransactionRepository
    {
            private readonly AppDbContext _dbContext;

            public TransactionRepository(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<IDbContextTransaction> BeginTransactionAsync()
            {
                return await _dbContext.Database.BeginTransactionAsync();
            }
            public async Task AddTransaction(Transaction transaction)
            {
                await _dbContext.Transactions.AddAsync(transaction);
            }

        }
}
