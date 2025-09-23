using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<bool> ExistsByIINAsync(string iin);
        Task SaveDataClientAsync(Client client);
        Task<int> DeleteAsync(string IIN);
        Task<Client?> FindByIINAsync(string IIN);
    }
}
