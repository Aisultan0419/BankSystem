using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task SaveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> FindRefreshToken(string refreshToken);
    }
}
