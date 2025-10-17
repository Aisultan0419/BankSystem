using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.TransactionDTO
{
    public class PurchaseQueryDTO
    {
        public string? lastNumbers { get; set; }
        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
        public required string StoreIban { get; set; }
    }
}
