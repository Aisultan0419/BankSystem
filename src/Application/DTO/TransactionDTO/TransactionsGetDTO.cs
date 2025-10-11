using Domain.Models;

namespace Application.DTO.TransactionDTO
{
    public class TransactionsGetDTO
    {
        public string? From { get; init; }
        public string? To { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; } = "KZT";
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string Type { get; set; } = null!;
    }
}
