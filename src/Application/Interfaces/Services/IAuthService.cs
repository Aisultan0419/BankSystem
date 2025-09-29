using Application.DTO;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginStatusDTO> LoginPin(string appUserId, string pinCode);
        Task<LoginStatusDTO> RefreshToken();
        Task<LoginStatusDTO> LoginPassword(string email, string password);
    }
}
