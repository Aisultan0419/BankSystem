using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly IDepositService _depositService;
        private readonly ITransferService _transferService;
        public TransactionController(IDepositService depositService, ITransferService transferService)
        {
            _depositService = depositService;
            _transferService = transferService;
        }
        [Authorize]
        [HttpPost("deposit")]
        public async Task<ActionResult<DepositResponseDTO>> Deposit([FromQuery] decimal amount, [FromQuery] string lastNumbers)
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return new DepositResponseDTO
                {
                    message = "User is not authenticated"
                };
            }
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _depositService.DepositAsync(amount, appUserIdClaim!, lastNumbers);
            if (result.depositedAmount == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpPost("transfer")]
        public async Task<ActionResult<TransferResponseDTO>> Transfer([FromQuery] string iban, [FromQuery] decimal amount,
            [FromQuery] string lastNumbers)
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return new TransferResponseDTO
                {
                    message = "User is not authenticated"
                };
            }
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _transferService.TransferAsync(appUserIdClaim!, iban, amount, lastNumbers);
            if (result.transferredAmount == null)
            {
                return BadRequest(result.message);
            }
            return Ok(result);
        }
    }
}
