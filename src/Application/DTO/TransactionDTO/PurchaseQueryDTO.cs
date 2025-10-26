using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.TransactionDTO
{
    public record PurchaseQueryDTO(
    string? lastNumbers,
    decimal Amount,
    Guid OrderId,
    string StoreIban
    );
}
