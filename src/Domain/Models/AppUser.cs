using Domain.Enums;

namespace Domain.Models
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public Client? Client { get; set; }
        public ICollection<RefreshToken> refreshTokens { get; set; } = new List<RefreshToken>();
    }
}
