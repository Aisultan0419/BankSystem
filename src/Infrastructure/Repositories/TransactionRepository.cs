using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Accounts;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Net;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

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
            public async Task AddOutbox(Outbox outBox)
            {
                await _dbContext.OutBoxes.AddAsync(outBox);
            }
            public async Task<bool> CheckMessageForIdempotency(Guid corId)
            {
                var res = await _dbContext.SavingsAccounts.AnyAsync(a => a.CorrelationId == corId);
                return res;
            }
             public async Task<CurrentAccount> GetCurrentAccountByClient(Client client)
            {
                var currentAccount = await _dbContext.Accounts
                                .OfType<CurrentAccount>()
                                .Where(a => a.ClientId == client.Id)
                                .FirstOrDefaultAsync();
                return currentAccount!;
            }
            public async Task AddAccrualInterestHistory(InterestAccrualHistory interestAccrualHistory)
            {
                await _dbContext.InterestAccrualHistory.AddAsync(interestAccrualHistory);
            }
            public bool IsUniqueViolationCheck(DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException pgEx)
                {
                    return pgEx.SqlState == "23505";
                }
                return false;
            }
    }
}
