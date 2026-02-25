using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.ClientDTO;

namespace Application.Interfaces.Services.Clients
{
    public interface IClientService
    {
        Task<RegistrationStatusDTO> Register(ClientCreateDTO clientDTO);
        Task<bool> DeleteClient(string iin);
    }
}
