namespace Domain.Models.Accounts
{
    public abstract class Account
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid ClientId { get; init; }
        public Client Client { get; set; } = null!;
        public string Iban { get; set; } = null!;

        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public ICollection<InterestAccrualHistory> AccrualHistory { get; private set; } = new List<InterestAccrualHistory>();
        public decimal Balance
        {
            get => _balance.Amount;
            protected set => _balance = new Money(value, Currency ?? "KZT");
        }
        protected Money _balance = new Money(0m, "KZT");
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
        public virtual void ResetLimit(DateOnly kazToday) { }
        public abstract void Deposit(decimal amount);
        public abstract void TransferOut(decimal amount);
    }
}
