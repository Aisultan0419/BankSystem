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
        public CardController(IGetCardService getCardService, IGetRequisitesOfCardService getRequisitesOfCard)
        {
            _getCardService = getCardService;
            _getRequisitesOfCardService = getRequisitesOfCard;
        }
        [Authorize]
        [HttpGet("getAllCards")]
        public async Task<ActionResult<IEnumerable<GetCardDTO>>> GetAllCards()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized("No token or not authenticated");

            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(appUserIdClaim, out var appUserGuid))
                return BadRequest("Invalid user id format");
            var cards = await _getCardService.GetAllCards(appUserIdClaim ?? throw new Exception("NoId"));
            if (cards != null)
            {
                return Ok(cards);
            }
            return NotFound();
        }
        [Authorize]
        [HttpGet("getRequisitesCards")]
        public async Task<ActionResult<CardRequisitesDTO>> GetRequisites([FromQuery] LastNumbersDTO lastNumbersDTO)
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized("No token or not authenticated");

            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var card = await _getRequisitesOfCardService.GetRequisitesOfCard(appUserIdClaim ?? throw new Exception("AppUser id is not here"), lastNumbersDTO.LastNumbers);
            if (card == null)
            {
                return NotFound("Card was not found");
            }
            return Ok(card);
        }
    }
    
}

