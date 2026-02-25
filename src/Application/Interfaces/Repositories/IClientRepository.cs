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
        Task<bool> ExistsByIinAsync(string iin);
        Task<int> DeleteAsync(string iin);
        Task<Client?> FindByIinAsync(string iin);
    }
}
