
namespace Application.MessageContracts
{
    public sealed class TransactionRequested : IOutboxMessage
    {
        public Guid MessageId { get; init; }
        public Guid CorrelationId { get; init; }
        public Guid UserId { get; init; }
        public decimal Amount { get; init; }
        public DateTime OccurredOn { get; init; }
    }
}
