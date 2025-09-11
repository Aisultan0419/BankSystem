using Domain.Enums;
using Domain.Models;

namespace Application.DTO
{
    public class AppUserCreateDTO
    {
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public required string IIN { get; set; }
    }
}
