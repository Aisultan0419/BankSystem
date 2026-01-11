using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.TransactionDTO
{
    public record SavingsRequestDTO(decimal Amount, string LastNumbers);
}
