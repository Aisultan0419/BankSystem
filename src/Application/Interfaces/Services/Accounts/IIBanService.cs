using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Interfaces.Services.Accounts
{
    public interface IIbanService
    {
        Task<string> GetIban(AccountType accountType, Guid id);
    }
}
