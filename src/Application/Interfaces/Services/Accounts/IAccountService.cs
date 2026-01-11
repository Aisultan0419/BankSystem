using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.Accounts;

namespace Application.Interfaces.Services.Accounts
{
    public interface IAccountService
    {
        Task<CurrentAccount> CreateAccount(Client client);
    }
}
