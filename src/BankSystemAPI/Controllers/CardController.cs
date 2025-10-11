using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTO.CardDTO;
using Application.Interfaces.Services;
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
            _logger.LogInformation("Fetching all cards for the authenticated user.");
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized("No token or not authenticated");
            _logger.LogInformation("User is authenticated.");
            _logger.LogInformation("Attempt to get id of user...");
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Attempt to get all cards of user...");
            var cards = await _getCardService.GetAllCards(appUserIdClaim ?? throw new Exception("NoId"));
            if (cards != null)
            {
                _logger.LogInformation("Cards retrieved successfully.");
                return Ok(cards);
            }
            _logger.LogWarning("No cards found for the user.");
            return NotFound();
        }
        [Authorize]
        [HttpGet("cards/requisites")]
        public async Task<ActionResult<CardRequisitesDTO>> GetRequisites([FromQuery] LastNumbersDTO lastNumbersDTO)
        {
            _logger.LogInformation("GetRequisites endpoint has started...");
            _logger.LogInformation("Fetching card requisites for the authenticated user.");
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized("No token or not authenticated");

            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var card = await _getRequisitesOfCardService.GetRequisitesOfCard(appUserIdClaim ?? throw new Exception("AppUser id is not here"), lastNumbersDTO.LastNumbers);
            if (card == null)
            {
                _logger.LogWarning("Card not found with the provided last numbers.");
                return NotFound("Card was not found");
            }
            _logger.LogInformation("Card requisites retrieved successfully.");
            return Ok(card);
        }

    }

}

