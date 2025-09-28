using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Application.DTO
{
    public class CardRequisitesDTO
    {
        public string full_name { get; init; } = null!;
        public string Pan { get; init; } = null!;
        public string? ExpiryDate { get; init; }
    }
}
