using Domain.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Transactions
{
    public interface ISavingAccountCreationTransaction
    {
        Task ProcessSavingAccountCreationTransaction(SavingAccount savingAccount);
    }
}
