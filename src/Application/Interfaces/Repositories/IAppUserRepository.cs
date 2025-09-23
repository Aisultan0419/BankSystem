using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IAppUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<AppUser?> GetAppUserByEmail(string email);
        Task<AppUser> GetAppUserAsync(Guid Id);
        Task SaveDataAppUserAsync(AppUser appUser);
    }
}
