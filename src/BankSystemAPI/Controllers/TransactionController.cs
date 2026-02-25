using Application.DTO.TransactionDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services.Transactions;

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
        public TransactionController(IDepositService depositService
            , ITransferService transferService
            , ILogger<TransactionController> logger
            , ITransactionsGetService transactionsGetService)
        {
            _depositService = depositService;
            _transferService = transferService;
            _logger = logger;
            _transactionsGetService = transactionsGetService;
        }
        [Authorize]
        [HttpPost("deposit")]
        public async Task<ActionResult<DepositResponseDTO>> Deposit([FromBody] DepositQueryDTO depositQueryDTO)
        {
            _logger.LogInformation("Deposit endpoint has started...");
            var result = await _depositService.DepositAsync(depositQueryDTO.Amount, depositQueryDTO.LastNumbers);
            if (result.DepositedAmount == null)
            {
                _logger.LogWarning("Deposit failed: {message}", result.Message);
                return BadRequest(result.Message);
            }
            _logger.LogInformation("Deposit successful: {depositedAmount}", result.DepositedAmount);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("transfer")]
        public async Task<ActionResult<TransferResponseDTO>> Transfer([FromBody] TransferQueryDTO transferQueryDTO)
        {
            _logger.LogInformation("Transfer endpoint has started...");
            var result = await _transferService.TransferAsync(
                 transferQueryDTO.Iban
                ,transferQueryDTO.Amount
                ,transferQueryDTO.LastNumbers);
            if (result.TransferredAmount == null)
            {
                _logger.LogWarning("Transfer failed: {message}", result.Message);
                return BadRequest(result.Message);
            }
            _logger.LogInformation("Transfer successful: {transferredAmount}", result.TransferredAmount);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<ActionResult<List<TransactionsGetDTO>>> GetTransactions([FromQuery] TransactionHistoryQueryDTO transactionHistoryQueryDTO)
        {
            _logger.LogInformation("GetTransactions endpoint has started...");
            var result = await _transactionsGetService.GetAllTransactionsAsync(transactionHistoryQueryDTO);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("GetTransactions failed: {message}", result.Message);
                return NotFound(result.Message);
            }
            _logger.LogInformation("GetTransactions successful: {count} transactions retrieved", result.Data?.Count ?? 0);
            return Ok(result.Data);
        }
    }
}
