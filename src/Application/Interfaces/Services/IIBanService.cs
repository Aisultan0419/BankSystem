using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Interfaces.Services
{
    public interface IIBanService
    {
        Task<string> GetIban(AccountType accountType, Guid Id);
        int Mod97(string input);
        string Digitalization(string letters);
    }
}
