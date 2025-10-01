using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.DTO.AuthDTO
{
    public class LoginStatusDTO
    {
        public string VerificationStatus { get; set; } = "NotStarted";
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public int ExpiresIn { get; set; }
        public Guid UserId { get; set; }
        public string? DeviceInfo { get; set; }
    }
}
