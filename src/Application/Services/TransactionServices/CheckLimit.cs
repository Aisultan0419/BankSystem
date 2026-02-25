using Domain.Models.Accounts;

namespace Application.Services.TransactionServices
{
    public class CheckLimit
    {
        private const decimal DailyLimit = 2_000_000m;
        private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");
        private DateTime KazNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        private DateOnly KazToday => DateOnly.FromDateTime(KazNow);
        public (bool isAllowed, decimal? availableAmount) CheckDailyTransferLimit(Account account, decimal amount)
        {
            if (account.LastTransferDateKz != KazToday)
            {
                account.ResetLimit(KazToday);
            }
            if (account.TransferredLastDay + amount > DailyLimit)
            {
                var canDeposit = DailyLimit - account.TransferredLastDay;
                return (false, canDeposit);
            }
            return (true, null);
        }
    }
}
