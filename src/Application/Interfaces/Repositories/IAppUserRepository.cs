
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IAppUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<AppUser?> GetAppUserByEmail(string email);
        Task<AppUser> GetAppUserAsync(Guid id);
    }
}
