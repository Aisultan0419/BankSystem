using Application.DTO;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginStatusDTO> Login(string email, string password);
        Task<LoginStatusDTO> RefreshToken();
    }
}
