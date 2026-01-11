using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.CardDTO;

namespace Application.Interfaces.Services
{
    public interface IGetCardService
    {
        Task<IEnumerable<GetCardDTO>> GetAllCards(string appUserId);
    }
}
