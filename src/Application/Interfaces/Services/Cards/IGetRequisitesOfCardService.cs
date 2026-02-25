using Application.DTO.CardDTO;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Cards
{
    public interface IGetRequisitesOfCardService
    {
        Task<ApiResponse<CardRequisitesDTO>> GetRequisitesOfCard(string last_numbers);
    }
}
