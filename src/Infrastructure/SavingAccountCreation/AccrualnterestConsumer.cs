
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Infrastructure.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SavingAccountCreation
{
    public class AccrualnterestConsumer : IConsumer<AccrueInterestCommand>
    {
        private readonly ILogger<AccrualnterestConsumer> _logger;
        private readonly IInterestAccrualService _interestAccrualService;
        public AccrualnterestConsumer(ILogger<AccrualnterestConsumer> logger, IInterestAccrualService interestAccrualService)
        {
            _logger = logger;
            _interestAccrualService = interestAccrualService;
        }
        public async Task Consume(ConsumeContext<AccrueInterestCommand> context)
        {
            var message = context.Message;
            try
            {
                await _interestAccrualService.AccrualInterest(message);
                _logger.LogInformation("Successfully processed interest accrual {AccountId}", message.AccountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process interest accrual {AccountId}", message.AccountId);
                throw;
            }
        }
    }
}
