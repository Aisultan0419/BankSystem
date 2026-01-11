using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Accounts
{
    public class InterestAccrualHistory
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public DateOnly AccrualDate { get; init; }
        public decimal AmountAccruedToday { get; init; }
        public decimal AmountAccruedApplied { get; init; }
        public DateTime CreatedAt { get; init; }
        public Account Account { get; set; } = null!;
    }
}
