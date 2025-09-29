using Microsoft.EntityFrameworkCore.Storage;
using Domain.Models;
namespace Application.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task AddTransaction(Transaction transaction);
    }
}
