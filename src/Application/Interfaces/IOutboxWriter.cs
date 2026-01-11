using Application.MessageContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOutboxWriter
    {
        Task Add<T>(T message) where T : IOutboxMessage;
    }
}
