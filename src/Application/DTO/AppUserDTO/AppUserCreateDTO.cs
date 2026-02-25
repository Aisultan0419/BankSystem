using Domain.Enums;
using Domain.Models;

namespace Application.DTO.AppUserDTO
{
    public record AppUserCreateDTO(
    string Email,
    string PasswordHash,
    string Iin,
    string PinCode);
}
