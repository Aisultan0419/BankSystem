namespace Domain.Models
{
    public class Account
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid ClientId { get; init; }
        public Client Client { get; set; } = null!;
        public string Iban { get; set; } = null!;


        public decimal Balance
        {
            get => _balance.Amount;
            private set => _balance = new Money(value, Currency ?? "KZT");
        }
        private Money _balance = new Money(0m, "KZT");
        public string? Currency { get; } = "KZT";
        public DateOnly LastDepositDateKz { get; set; }
        public Money? DepositedLastDayMoney { get; set; }
        public decimal? DepositedLastDay
        {
            get => DepositedLastDayMoney?.Amount;
            set => DepositedLastDayMoney = value.HasValue ? new Money(value.Value, Currency ?? "KZT") : null;
        }
        public DateOnly LastTransferDateKz { get; set; }
        public Money? TransferredLastDayMoney { get; set; }
        public decimal? TransferredLastDay
        {
            get => TransferredLastDayMoney?.Amount;
            set => TransferredLastDayMoney = value.HasValue ? new Money(value.Value, Currency ?? "KZT") : null;
        }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public void Deposit(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            _balance = _balance + m;
        }
        public void TransferOut(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            _balance = new Money(_balance.Amount - m.Amount, _balance.Currency);
        }
    }
}
