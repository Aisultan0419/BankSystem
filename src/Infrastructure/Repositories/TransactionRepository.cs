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
            public IExecutionStrategy CreateExecutionStrategy()
            {
                return _dbContext.Database.CreateExecutionStrategy();
            }

        public async Task AddTransaction(Transaction transaction)
            {
                await _dbContext.Transactions.AddAsync(transaction);
            }
            public async Task<List<TransactionsGetDTO>> GetTransactions(Client client, TransactionHistoryQueryDTO thqDTO)
            {
                var query = _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.ClientId == client.Id);
            if (thqDTO.startDate.HasValue)
            {
                var start = DateTime.SpecifyKind(thqDTO.startDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(t => t.CreatedAt >= start);
            }

            if (thqDTO.endDate.HasValue)
            {
                var exclusiveEnd = DateTime.SpecifyKind(thqDTO.endDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(t => t.CreatedAt < exclusiveEnd);
            }

            if (thqDTO.pageNumber.HasValue && thqDTO.pageSize.HasValue)
                {
                    query = query
                    .Skip((thqDTO.pageNumber.Value - 1) * thqDTO.pageSize.Value)
                    .Take(thqDTO.pageSize.Value);
                }
                var result = await query
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
