using Application.DTO.TransactionDTO;
using Application.Interfaces.Services;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Application.Responses;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly IDepositService _depositService;
        private readonly ITransferService _transferService;
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionsGetService _transactionsGetService;
        private readonly IPurchaseService _purchaseService;
        public TransactionController(IDepositService depositService
            , ITransferService transferService
            , ILogger<TransactionController> logger
            , ITransactionsGetService transactionsGetService
            , IPurchaseService purchaseService)
        {
            _depositService = depositService;
            _transferService = transferService;
            _logger = logger;
            _transactionsGetService = transactionsGetService;
            _purchaseService = purchaseService;
        }
        [Authorize]
        [HttpPost("deposit")]
        public async Task<ActionResult<DepositResponseDTO>> Deposit([FromBody] DepositQueryDTO depositQueryDTO)
        {
            _logger.LogInformation("Deposit endpoint has started...");
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
            _logger.LogInformation("Transfer endpoint has started...");
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
                , transferQueryDTO.Iban
                , transferQueryDTO.Amount
                , transferQueryDTO.LastNumbers);
            if (result.transferredAmount == null)
            {
                _logger.LogWarning("Transfer failed: {message}", result.message);
                return BadRequest(result.message);
            }
            _logger.LogInformation("Transfer successful: {transferredAmount}", result.transferredAmount);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<ActionResult<List<TransactionsGetDTO>>> GetTransactions([FromQuery] TransactionHistoryQueryDTO transactionHistoryQueryDTO)
        {
            _logger.LogInformation("GetTransactions endpoint has started...");
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is not authenticated.");
                return new List<TransactionsGetDTO>();
            }
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _transactionsGetService.GetAllTransactionsAsync(appUserIdClaim!, transactionHistoryQueryDTO);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("GetTransactions failed: {message}", result.Message);
                return NotFound(result.Message);
            }
            _logger.LogInformation("GetTransactions successful: {count} transactions retrieved", result.Data?.Count ?? 0);
            return Ok(result.Data);
        }
        [Authorize]
        [HttpPost("purchase")]
        public async Task<ActionResult<OrderResponse<PurchaseResponseDTO>>> Purchase([FromBody] PurchaseQueryDTO purchaseQueryDTO)
        {
            _logger.LogInformation("Purchase endpoint has started...");
            var appUserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _purchaseService.PurchaseAsync(purchaseQueryDTO, appUserIdClaim!);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Purchase failed: {message}", result.Message);
                return BadRequest(new OrderResponse<PurchaseResponseDTO>
                {
                    isSuccess = false,
                    Message = result.Message!,
                    Data = null
                });
            }
            _logger.LogInformation("Purchase successful for OrderId: {orderId}", purchaseQueryDTO.OrderId);
            return Ok(result);
        }
    }
}
