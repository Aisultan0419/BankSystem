using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Domain.Models;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Application.DTO.TransactionDTO;

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
            public async Task<List<TransactionsGetDTO>> GetTransactions(Client client)
            {
                var result = await _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.ClientId == client.Id)
                .Select(t => new TransactionsGetDTO
                {
                    From = t.From,
                    To = t.To,
                    Amount = t.Amount,
                    CreatedAt = t.CreatedAt,
                    Type = t.Type
                }) 
                .ToListAsync();

                return result;
            }

        }
}
