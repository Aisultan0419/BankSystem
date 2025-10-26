namespace Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; init; }

        public required string From { get; init; }   
        public required string To { get; init; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public decimal Amount
        {
            get => AmountMoney.Amount;
            init => AmountMoney = new Money(value, Currency ?? "KZT");
        }
        public Money AmountMoney { get; init; } = new Money(0m, "KZT");
        public string Currency => AmountMoney.Currency;

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string Type { get; set; } = null!;

    } 
}
