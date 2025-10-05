using Application.DTO.TransactionDTO;
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
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(IDepositService depositService
            ,ITransferService transferService
            ,ILogger<TransactionController> logger)
        {
            _depositService = depositService;
            _transferService = transferService;
            _logger = logger;
        }
        [Authorize]
        [HttpPost("deposit")]
        public async Task<ActionResult<DepositResponseDTO>> Deposit([FromBody] DepositQueryDTO depositQueryDTO)
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is not authenticated.");
                return new DepositResponseDTO
                { 
                    message = "User is not authenticated"
                };
            }
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _depositService.DepositAsync(depositQueryDTO.Amount, appUserIdClaim!, depositQueryDTO.LastNumbers);
            if (result.depositedAmount == null)
            {
                _logger.LogWarning("Deposit failed: {message}", result.message);
                return BadRequest(result.message);
            }
            _logger.LogInformation("Deposit successful: {depositedAmount}", result.depositedAmount);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("transfer")]
        public async Task<ActionResult<TransferResponseDTO>> Transfer([FromBody] TransferQueryDTO transferQueryDTO)
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is not authenticated.");
                return new TransferResponseDTO
                {
                    message = "User is not authenticated"
                };
            }
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _transferService.TransferAsync(appUserIdClaim!
                ,transferQueryDTO.Iban
                ,transferQueryDTO.Amount
                ,transferQueryDTO.LastNumbers);
            if (result.transferredAmount == null)
            {
                _logger.LogWarning("Transfer failed: {message}", result.message);
                return BadRequest(result.message);
            }
            _logger.LogInformation("Transfer successful: {transferredAmount}", result.transferredAmount);
            return Ok(result);
        }
    }
}
