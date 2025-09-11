using Domain.Enums;
using Domain.Models;

namespace Application.DTO
{
    public class ClientCreateDTO
    {
        public string? IIN { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
