using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public Guid UserId { get; set; }
        public DateTime RevokedAt { get; set; }
        public string? DeviceInfo { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
