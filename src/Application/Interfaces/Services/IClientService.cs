using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interfaces.Services
{
    public interface IClientService
    {
        Task<RegistrationStatusDTO> Register(ClientCreateDTO ClientDTO);
        Task<bool> DeleteClient(string IIN);
    }
}
