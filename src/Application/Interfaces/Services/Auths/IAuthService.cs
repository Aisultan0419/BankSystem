using Application.DTO.AuthDTO;

namespace Application.Interfaces.Services.Auths
{
    public interface IAuthService
    {
        Task<LoginStatusDTO> LoginPin(string appUserId, string pinCode);
        Task<LoginStatusDTO> RefreshToken();
        Task<LoginStatusDTO> LoginPassword(string email, string password);
    }
}
