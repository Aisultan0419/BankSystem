namespace Domain.Models
{
    public class Account
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid ClientId { get; init; }
        public Client Client { get; set; } = null!;
        public string Iban { get; set; } = null!;
        public decimal Balance { get; private set; } = 0;
        public string? Currency { get; } = "KZT";
        public DateOnly LastDepositDateKz { get; set; }
        public decimal? DepositedLastDay { get; set; }
        public DateOnly LastTransferDateKz { get; set; }
        public decimal? TransferredLastDay { get; set; }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public void Deposit(decimal amount)
        {
            Balance += amount;
        }
        public void TransferOut(decimal amount)
        {
            Balance -= amount;
        }
    }
}
