using Application.DTO.TransactionDTO;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Domain.Models;
using Domain.Models.Accounts;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class AddingOutboxTransaction : IAddingOutboxTransaction
    {
        private readonly ITransactionRepository _tR;
        private readonly IOutboxWriter _outboxWriter;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExecutionStrategyRunner _exRunner;
        private readonly ILogger<AddingOutboxTransaction> _logger;
        public AddingOutboxTransaction(
            ITransactionRepository tr,
            IOutboxWriter outboxWriter,
            IClock clock,
            IUnitOfWork unitOfWork,
            IExecutionStrategyRunner exRunner,
            ILogger<AddingOutboxTransaction> logger)
        {
            _unitOfWork = unitOfWork;
            _tR = tr;
            _outboxWriter = outboxWriter;
            _clock = clock;
            _exRunner = exRunner;
            _logger = logger;
        }

        public async Task<Guid> ProcessOutboxAddingTransaction(Guid appUserId, SavingsRequestDTO savReqDTO)
        {
            Guid correlationId = Guid.NewGuid();
            await _exRunner.ExecuteAsync(async () =>
            {
                await using var tx = await _tR.BeginTransactionAsync();

                try
                {
                    var message = new TransactionRequested
                    {
                        MessageId = Guid.NewGuid(),
                        CorrelationId = correlationId,
                        UserId = appUserId,
                        Amount = savReqDTO.Amount,
                        OccurredOn = _clock.UtcNow
                    };

                    await _outboxWriter.Add(message);
                    await _unitOfWork.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger.LogError(ex, "Error processing outbox transaction.");
                    throw;
                }
            });
            return correlationId;
        }
    }

}
