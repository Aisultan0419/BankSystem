using Domain.Enums;
using Domain.Models;

namespace Application.DTO
{
    public class ClientCreateDTO
    {
        public required string IIN { get; set; } 
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
