
namespace Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; init; }
        public required string Token { get; init; }
        public DateTime Expires { get; init; } 
        public Guid UserId { get; init; }  
        public DateTime RevokedAt { get; set; }
        public string? DeviceInfo { get; set; }
        public required AppUser AppUser { get; init; }
    }
}
