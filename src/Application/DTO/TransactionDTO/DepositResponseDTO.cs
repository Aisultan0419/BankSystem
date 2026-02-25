using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.TransactionDTO
{
    public class DepositResponseDTO {
        public decimal? DepositedAmount { get; set; } 
        public string? Message { get; set; }
        public decimal? NewBalance { get; set; } 
    }
}
