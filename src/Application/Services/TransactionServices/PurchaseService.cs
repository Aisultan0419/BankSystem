using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Transactions;
using Application.Responses;
using System.Net.Http.Json;

namespace Application.Services.TransactionServices
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CheckLimit _checkLimit;
        public PurchaseService(
            IAccountRepository accountRepository,
            IAppUserRepository appUserRepository,
            IHttpClientFactory httpClientFactory,
            CheckLimit checkLimit,
            ITransactionProcessor transactionProcessor
            )
        {
            _accountRepository = accountRepository;
            _appUserRepository = appUserRepository;
            _httpClientFactory = httpClientFactory;
            _checkLimit = checkLimit;
            _transactionProcessor = transactionProcessor;
        }
        public async Task<ApiResponse<PurchaseResponseDTO>> PurchaseAsync(PurchaseQueryDTO query, string appUserId)
        {
            Guid.TryParse(appUserId, out var appUserGuid);
            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);
            if (appUser == null)
            {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = "AppUser was not found",
                    Data = null
                };
            }
            var client = appUser.Client;
            var card = await _accountRepository.GetRequisitesDTOAsync(client.Id, query.lastNumbers!);
            if (card == null)
            {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Card was not found",
                    Data = null
                };
            }
            var fromAccount = await _accountRepository.GetAccountById(card.AccountId);
            if(fromAccount == null)
                {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Account was not found",
                    Data = null
                };
            }
            if (fromAccount.Balance < query.Amount)
            {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Insufficient funds",
                    Data = null
                };
            }
            var clientHttp = _httpClientFactory.CreateClient();
            var checkUrl = "http://localhost:5155/api/Order/order/check";
            var checkRequest = new { OrderId = query.OrderId, Amount = query.Amount };
            var response = await (await clientHttp.PostAsJsonAsync(checkUrl, checkRequest))
                .Content.ReadFromJsonAsync<OrderResponse<PurchaseResponseDTO>>();

            if (response!.isSuccess == false)
            {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = response.Message,
                    Data = null
                };
            }

            var checkLimit =  _checkLimit.checkLimit(fromAccount, query.Amount);

            if (checkLimit.Item1 == false)
            {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = $"You can purchase only {checkLimit.Item2} for today",
                    Data = null
                };
            }
            var toAccount = await _accountRepository.GetAccountByIban(query.StoreIban);
            if (toAccount == null)
            {
                return new ApiResponse<PurchaseResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Store account was not found",
                    Data = null
                };
            }
            await _transactionProcessor.ProcessTransferAsync(appUser, fromAccount, toAccount, query.Amount);
            return new ApiResponse<PurchaseResponseDTO>
            {
                IsSuccess = true,
                Message = "Purchase completed successfully",
                Data = new PurchaseResponseDTO
                {
                    OrderId = query.OrderId,
                    Amount = query.Amount,
                    PaymentType = response?.Data?.PaymentType
                }
            };
        }
    }
}
