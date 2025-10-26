using Domain.Enums;
using Domain.Models;

namespace Application.DTO.ClientDTO
{
    public record ClientCreateDTO(
    string IIN,
    string FullName,
    string? PhoneNumber
);
}
