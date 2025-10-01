using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTO.CardDTO
{
    public class GetCardDTO
    {
        public string? PanMasked { get; init; }
        public string? Status { get; init; }
        public decimal Balance { get; init; }
        public string? Currency { get; } = "KZT";

    }
}
