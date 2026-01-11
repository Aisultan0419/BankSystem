using Domain.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.TransactionServices
{
    public class CheckLimit
    {
        private const decimal daily_limit = 2000000m;
        private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");
        private DateTime KazNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        private DateOnly KazToday => DateOnly.FromDateTime(KazNow);
        public (bool, decimal?) checkLimit(Account account, decimal amount)
        {
            if (account.LastTransferDateKz != KazToday)
            {
                account.TransferredLastDay = 0m;
                account.LastTransferDateKz = KazToday;
            }
            if (account.TransferredLastDay + amount > daily_limit)
            {
                var canDeposit = daily_limit - account.TransferredLastDay;
                return (false, canDeposit);
            }
            return (true, null);
        }
    }
}
