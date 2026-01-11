using Application.DTO.TransactionDTO;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Domain.Models;
using Domain.Models.Accounts;
using System.Runtime.CompilerServices;

namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class AddingOutboxTransaction : IAddingOutboxTransaction
    {
        private readonly ITransactionRepository _tR;
        private readonly IOutboxWriter _outboxWriter;
        private readonly IClock _clock;
        private readonly IUserRepository _userRepository;
        private readonly IExecutionStrategyRunner _exRunner;

        public AddingOutboxTransaction(
            ITransactionRepository tr,
            IOutboxWriter outboxWriter,
            IClock clock,
            IUserRepository userRep,
            IExecutionStrategyRunner exRunner)
        {
            _userRepository = userRep;
            _tR = tr;
            _outboxWriter = outboxWriter;
            _clock = clock;
            _exRunner = exRunner;
        }

        public async Task<Guid> ProcessOutboxAddingTransaction(Guid appUserId, SavingsRequestDTO savReqDTO)
        {
            Guid correlationId = Guid.Empty;
            await _exRunner.ExecuteAsync(async () =>
            {
                await using var tx = await _tR.BeginTransactionAsync();

                var correlationId = Guid.NewGuid();
                var message = new TransactionRequested
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = correlationId,
                    UserId = appUserId,
                    Amount = savReqDTO.Amount,
                    OccurredOn = _clock.UtcNow
                };

                await _outboxWriter.Add(message);

                await _userRepository.SaveChangesAsync();
                await tx.CommitAsync();
            });
            return correlationId;
        }
    }

}
