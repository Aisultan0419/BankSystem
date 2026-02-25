using Application.DTO.CardDTO;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/card")]
    public class CardController : ControllerBase
    {
        private readonly IGetCardService _getCardService;
        private readonly IGetRequisitesOfCardService _getRequisitesOfCardService;
        private readonly ILogger<CardController> _logger;
        public CardController(IGetCardService getCardService, IGetRequisitesOfCardService getRequisitesOfCard, ILogger<CardController> logger)
        {
            _getCardService = getCardService;
            _getRequisitesOfCardService = getRequisitesOfCard;
            _logger = logger;
        }
        [Authorize]
        [HttpGet("cards")]
        public async Task<ActionResult<IEnumerable<GetCardDTO>>> GetAllCards()
        {
            _logger.LogInformation("GetAllCards endpoint has started...");
            _logger.LogInformation("Attempt to get all cards of user...");
            var result = await _getCardService.GetAllCards();
            if (result.IsSuccess is true)
            {
                _logger.LogInformation(result.Message);
                return Ok(result.Data);
            }
            _logger.LogWarning(result.Message);
            return NotFound(result.Error);
        }
        [Authorize]
        [HttpGet("cards-requisites")]
        public async Task<ActionResult<CardRequisitesDTO>> GetRequisites([FromQuery] LastNumbersDTO lastNumbersDTO)
        {
            _logger.LogInformation("GetRequisites endpoint has started...");
            var result = await _getRequisitesOfCardService.GetRequisitesOfCard(lastNumbersDTO.LastNumbers);
            if (result.IsSuccess is false)
            {
                _logger.LogWarning(result.Message);
                return NotFound(result.Error);
            }
            _logger.LogInformation(result.Message);
            return Ok(result.Data);
        }
    }
}

