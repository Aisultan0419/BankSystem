using Domain.Models;

namespace Application.DTO.TransactionDTO
{
    public record TransactionsGetDTO(
    string? From,
    string? To,
    decimal Amount,
    string Currency = "KZT",
    DateTime CreatedAt = default,
    string Type = ""
)
    {
        public TransactionsGetDTO() : this(null, null, 0m, "KZT", DateTime.UtcNow, "") { }
    }
}
