using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Pan
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid CardId { get; set; }
        public Card Card { get; set; } = null!;
        
        public byte[] CipherText { get; set; } = null!;
        public byte[] Nonce { get; set; } = null!;
        public byte[] Tag { get; set; } = null!;
    }
}
