
namespace Domain.Models.Accounts
{
    public sealed class SavingAccount : Account
    {
        public InterestRate? _interestRate;
        public SavingAccount()
        {
        }
        public void Initialize(InterestRate interestRate)
        {
            _interestRate = interestRate;
        }
        public decimal AccruedInterest
        {
            get => _accruedInterest.Amount;
            private set => _accruedInterest = new Money(value, Currency ?? "KZT");
        }
        public Guid? CorrelationId { get; set; }
        private Money _accruedInterest = new Money(0m, "KZT");
        public decimal MinimumSavingAmount
        {
            get => _minimumSavingAmount.Amount;
        }
        private Money _minimumSavingAmount = new Money(1000m, "KZT");
        public void UpdateInterest(decimal interest)
        {
            var m = new Money(interest, Currency ?? "KZT");
            _accruedInterest += m;
        }
        public void ApplyAccruedInterest()
        {
            _balance += _accruedInterest;
            _accruedInterest = new Money(0m, Currency ?? "KZT");
        }
        public override void Deposit(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            _balance += m;
        }
        public override void TransferOut(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            if ((_balance - m).Amount < MinimumSavingAmount)
            {
                throw new InvalidOperationException("Cannot transfer out. Balance would fall below the minimum saving amount.");
            }
            _balance -= m;
        }
    }
}
