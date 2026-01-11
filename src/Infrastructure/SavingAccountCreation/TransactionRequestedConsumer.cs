using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;


namespace Infrastructure.MessageBroker
{
    public class TransactionRequestedConsumer : IConsumer<TransactionRequested>
    {
        private readonly ISavingAccountService _savingAccountService;
        private readonly ILogger<TransactionRequestedConsumer> _logger;

        public TransactionRequestedConsumer(ISavingAccountService savingAccountService, ILogger<TransactionRequestedConsumer> logger)
        {
            _savingAccountService = savingAccountService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransactionRequested> context)
        {
            var message = context.Message;

            try
            {
                await _savingAccountService.CreateSavingAccount(message);
                _logger.LogInformation("Successfully processed transaction {CorrelationId}", message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process transaction {CorrelationId}", message.CorrelationId);
                throw; 
            }
        }
    }

}
