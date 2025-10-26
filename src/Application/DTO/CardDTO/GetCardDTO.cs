using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTO.CardDTO
{
    public record GetCardDTO(
        string? PanMasked,
        string? Status,
        decimal Balance,
        string? Currency = "KZT"
    );
}
