using Application.DTO.TransactionDTO;
using Domain.Models;
using Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace Application.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task AddTransaction(Transaction transaction);
        Task<List<TransactionsGetDTO>> GetTransactions(Client client, TransactionHistoryQueryDTO thqDTO);
        IExecutionStrategy CreateExecutionStrategy();
        Task AddOutbox(Outbox outBox);
        Task<bool> CheckMessageForIdempotency(Guid corId);
        Task<CurrentAccount> GetCurrentAccountByClient(Client client);
        Task AddAccrualInterestHistory(InterestAccrualHistory interestAccrualHistory);
        bool IsUniqueViolationCheck(DbUpdateException ex);
    }
}
