using Application.DTO.TransactionDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Application.Responses;
using System.Security.Claims;
using Application.Interfaces.Services.Transactions;

namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISavingAccountService _savAccountService;
        public AccountController(ILogger<AccountController> logger, ISavingAccountService savAccountService)
        {
            _logger = logger;
            _savAccountService = savAccountService;
        }
        [Authorize]
        [HttpPost("saving-account")]
        public async Task<ActionResult<ApiResponse<SavingAccountResponseDTO>>> SavingAccountEndpoint([FromBody] SavingsRequestDTO savingReqDTO)
        {
            _logger.LogInformation("Saving account process endpoint has started...");
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _savAccountService.CreateSavingAccountAsync(appUserIdClaim!, savingReqDTO);
            if (result.IsSuccess is false)
            {
                _logger.LogWarning("Saving account process failed: {message}", result.Message);
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
