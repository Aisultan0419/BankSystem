using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Models;

namespace Application.Interfaces.Services
{
    public interface IUserRegisterService
    {
        Task<RegistrationStatusDTO> Register(ClientCreateDTO ClientDTO);
        Task<bool> DeleteClient(string IIN);
        Task<RegistrationStatusDTO> RegisterAppUser(AppUserCreateDTO AppUserDTO);
        Task<LoginStatusDTO> Login(string email, string password);
        Task<LoginStatusDTO> RefreshToken();
    }
}
