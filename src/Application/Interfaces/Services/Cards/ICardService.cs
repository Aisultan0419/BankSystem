using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Services.Cards
{
    public interface ICardService
    {
        Card CreateCard(Client client);
    }
}
