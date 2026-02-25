using Application.DTO.AuthDTO;

namespace Application.Interfaces.Services.Auths
{
    public interface IAuthService
    {
        Task<LoginStatusDTO> LoginProcess(string appUserId, string input);
        Task<LoginStatusDTO> RefreshToken();
    }
}
