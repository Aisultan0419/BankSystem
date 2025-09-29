namespace Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; init; }

        public required string From { get; init; }   
        public required string To { get; init; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public decimal Amount { get; init; }
        public string Currency { get; } = "KZT";

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string Type { get; set; } = null!;

    }
}
