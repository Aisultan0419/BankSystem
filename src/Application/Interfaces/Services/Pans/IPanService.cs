using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Services.Pans
{
    public interface IPanService
    {
        string CreatePan(Client client);
        string ControlNumber(string BIN, string card_number);
    }
}
