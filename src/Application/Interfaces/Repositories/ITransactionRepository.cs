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
        Task<List<TransactionsGetDTO>> GetTransactions(Client client, TransactionHistoryQueryDTO thqDTO);
        IExecutionStrategy CreateExecutionStrategy();
        Task<bool> CheckMessageForIdempotency(Guid corId);
        Task<CurrentAccount> GetCurrentAccountByClient(Client client);
        bool IsUniqueViolationCheck(DbUpdateException ex);
    }
}
