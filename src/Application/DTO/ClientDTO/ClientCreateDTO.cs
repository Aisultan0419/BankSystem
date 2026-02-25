using Domain.Enums;
using Domain.Models;

namespace Application.DTO.ClientDTO
{
    public record ClientCreateDTO(
    string Iin,
    string FullName,
    string? PhoneNumber
);
}
