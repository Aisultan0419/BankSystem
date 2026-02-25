using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.AppUserDTO;
using Application.DTO.ClientDTO;

namespace Application.Interfaces.Services.AppUsers
{
    public interface IAppUserService
    {
        Task<RegistrationStatusDTO> RegisterAppUser(AppUserCreateDTO appUserDTO);
    }
}
