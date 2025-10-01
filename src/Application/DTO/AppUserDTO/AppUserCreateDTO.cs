using Domain.Enums;
using Domain.Models;

namespace Application.DTO.AppUserDTO
{
    public class AppUserCreateDTO
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string IIN { get; set; }
        public string PinCode { get; set; } = null!;    
    }
}
