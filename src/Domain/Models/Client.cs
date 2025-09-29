using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Models
{
    public class Client
    {
        public Guid Id { get; init; }
        public required string IIN { get; init; }
        public KycStatus KycStatus { get; set; }
        public string? FullName { get; set; }
        public bool IsDeleted { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Account> Accounts { get; } = new List<Account>();
        public ICollection<AppUser> AppUsers { get; } = new List<AppUser>();
    }
}
