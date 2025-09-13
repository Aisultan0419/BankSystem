using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByIINAsync(string iin);
        Task SaveDataClientAsync(Client client);
        Task SaveDataAppUserAsync(AppUser appUser);
        Task SaveChangesAsync();
        Task<int> DeleteAsync(string IIN);
        Task<Client?> FindByIINAsync(string IIN);
        Task<bool> ExistsByEmailAsync(string email);
        Task<AppUser?> GetAppUserByEmail(string email);
        Task SaveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> FindRefreshToken(string refreshToken);
        Task<AppUser> GetAppUserAsync(Guid Id);
    }
}
