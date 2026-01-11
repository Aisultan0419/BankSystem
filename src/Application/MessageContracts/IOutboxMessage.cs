using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MessageContracts
{
    public interface IOutboxMessage
    {
        public Guid MessageId { get; }
    }
}
