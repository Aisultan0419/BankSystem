
namespace Domain.Models
{
    public class Outbox
    {
        public Guid Id { get; init; }
        public string EventType { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime OccurredOn { get; init; }
        public DateTime? ProcessedOn { get; set; }
        public int RetryCount { get; set; }
    }
}
