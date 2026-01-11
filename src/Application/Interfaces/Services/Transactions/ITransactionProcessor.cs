using Domain.Models.Accounts;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Transactions
{
    public interface ITransactionProcessor
    {
        Task ProcessTransferAsync(AppUser appUser, Account fromAccount, Account toAccount, decimal amount);
    }
}
