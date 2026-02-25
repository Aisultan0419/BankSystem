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

            public async Task<List<TransactionsGetDTO>> GetTransactions(Client client, TransactionHistoryQueryDTO thqDTO)
            {
                var query = _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.ClientId == client.Id);
            if (thqDTO.StartDate.HasValue)
            {
                var start = DateTime.SpecifyKind(thqDTO.StartDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(t => t.CreatedAt >= start);
            }

            if (thqDTO.EndDate.HasValue)
            {
                var exclusiveEnd = DateTime.SpecifyKind(thqDTO.EndDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(t => t.CreatedAt < exclusiveEnd);
            }

            if (thqDTO.PageNumber.HasValue && thqDTO.PageSize.HasValue)
                {
                    query = query
                    .Skip((thqDTO.PageNumber.Value - 1) * thqDTO.PageSize.Value)
                    .Take(thqDTO.PageSize.Value);
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
