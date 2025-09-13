using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Auth
{
    public interface IRefreshTokenProvider
    {
        RefreshToken GenerateRefreshToken(Guid userId);
        string RefreshTokenHasher(string Token);
    }
}
