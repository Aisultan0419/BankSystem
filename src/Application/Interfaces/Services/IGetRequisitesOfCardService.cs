using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interfaces.Services
{
    public interface IGetRequisitesOfCardService
    {
        Task<CardRequisitesDTO> GetRequisitesOfCard(string appUserId, string last_numbers);
    }
}
