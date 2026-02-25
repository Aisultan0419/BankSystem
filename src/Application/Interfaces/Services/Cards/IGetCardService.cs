using Application.DTO.CardDTO;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IGetCardService
    {
        Task<ApiResponse<IEnumerable<GetCardDTO>>> GetAllCards();
    }
}
