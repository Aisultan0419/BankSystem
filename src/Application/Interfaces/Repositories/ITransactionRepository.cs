using Microsoft.EntityFrameworkCore.Storage;
using Domain.Models;
using Application.DTO.TransactionDTO;
namespace Application.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task AddTransaction(Transaction transaction);
        Task<List<TransactionsGetDTO>> GetTransactions(Client client);
    }
}
