using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task CreateAccount(Client client);
    }
}
