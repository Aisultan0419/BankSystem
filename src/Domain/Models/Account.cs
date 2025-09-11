namespace Domain.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Client? Client { get; set; }
        public string? Iban { get; set; }
        public decimal Balance { get; set; }
        public string? Currency { get; set; }
        public ICollection<Card>? Cards { get; set; }
    }
}
