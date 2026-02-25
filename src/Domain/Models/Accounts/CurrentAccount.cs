
namespace Domain.Models.Accounts
{
    public sealed class CurrentAccount : Account
    {
        public override void Deposit(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            _balance += m;
        }
        public override void TransferOut(decimal amount)
        {
            var m = new Money(amount, Currency ?? "KZT");
            _balance -= m;
        }
        public override void ResetLimit(DateOnly kazToday)
        {
            this.TransferredLastDay = 0m;
            this.LastTransferDateKz = kazToday;
        }
    }
}
