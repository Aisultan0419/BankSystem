using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task AddEncyptedPan(Pan pan);
        Task AddCard(Card card);
        Task AddAccount(Account account);
    }
}
