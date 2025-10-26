using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.TransactionDTO
{
    public class DepositResponseDTO {
        public decimal? depositedAmount { get; set; } 
        public string? message { get; set; }
        public decimal? newBalance { get; set; } 
    }
}
