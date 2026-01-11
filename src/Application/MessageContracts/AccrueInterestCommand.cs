using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MessageContracts
{
    public record AccrueInterestCommand(
    Guid AccountId
    );
}
