using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class TransferResponseDTO
    {
        public string? Full_name { get; set; }
        public decimal? transferredAmount { get; set; }
        public decimal? remainingBalance { get; set; }
        public string? message { get; set; }
    }
}
