using Domain.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.TransactionServices
{
    public class SystemClock : IClock 
    { 
        public DateTime UtcNow => DateTime.UtcNow; 
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;    
    }
}
