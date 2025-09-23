    using Domain.Enums;

    namespace Domain.Models
    {
        public class AppUser
        {
            public Guid Id { get; init; }
            public required string Email { get; set; }
            public required string PasswordHash { get; set; }
            public bool IsActive { get; set; }
            public VerificationStatus VerificationStatus { get; set; }
            public required Client Client { get; init; }
            public ICollection<RefreshToken> refreshTokens { get; set; } = new List<RefreshToken>();
        }
    }
