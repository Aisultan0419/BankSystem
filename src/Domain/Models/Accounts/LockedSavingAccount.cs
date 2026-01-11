using Domain.Enums;
namespace Domain.Models.Accounts
{
    public sealed class LockedSavingAccount : Account
    {
        private InterestRate? _interestRate;
        private IClock? _clock;
        public LockedSavingAccount() { } 
        public void Initialize(InterestRate interestRate, IClock clock)
        {
            _interestRate = interestRate;
            _clock = clock;
        }
        public decimal AccruedInterest
        {
            get => _accruedInterest.Amount;
            private set => _accruedInterest = new Money(value, Currency ?? "KZT");
        }
        private Money _accruedInterest = new Money(0m, "KZT");
        public decimal MinimumSavingAmount
        {
            get => _minimumSavingAmount.Amount; //fixed
        }
        private Money _minimumSavingAmount = new Money(100000m, "KZT");
        public override void Deposit(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            _balance += m;
        }
        public override void TransferOut(decimal amount)
        {
            switch (_interestRate!.Type)
            {
                case SavingType.Year:
                    if (_clock!.UtcNow < _interestRate.EffectiveFrom.ToDateTime(TimeOnly.MinValue).AddYears(1))
                        throw new InvalidOperationException("Cannot withdraw from a 1-year locked saving account before maturity.");
                    break;
                case SavingType.six_months:
                    if (_clock!.UtcNow < _interestRate.EffectiveFrom.ToDateTime(TimeOnly.MinValue).AddMonths(6))
                        throw new InvalidOperationException("Cannot withdraw from a 6-month locked saving account before maturity.");
                    break;

                case SavingType.three_months:
                    if (_clock!.UtcNow < _interestRate.EffectiveFrom.ToDateTime(TimeOnly.MinValue).AddMonths(3))
                        throw new InvalidOperationException("Cannot withdraw from a 3-month locked saving account before maturity.");
                    break;
                case SavingType.one_month:
                    if (_clock!.UtcNow < _interestRate.EffectiveFrom.ToDateTime(TimeOnly.MinValue).AddMonths(1))
                        throw new InvalidOperationException("Cannot withdraw from a 1-month locked saving account before maturity.");
                    break;

                default:
                    throw new InvalidOperationException("Unknown saving type.");
                
            }
            var m = new Money(amount, Currency ?? "KZT");
            _balance -= m;
        }
        
    }
}
