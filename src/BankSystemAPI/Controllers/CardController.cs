using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTO;
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
        public CardController(IGetCardService getCardService)
        {
            _getCardService = getCardService;
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
    }
    
}

