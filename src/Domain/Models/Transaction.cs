namespace Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Account? Account { get; set; }
        public int Amount { get; set; }
        public string? Currency { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? IdempotencyKey { get; set; }
        
    }
}
