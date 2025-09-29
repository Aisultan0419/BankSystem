using Domain.Enums;
namespace Domain.Models
{
    public class Card
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;
        public string? PanMasked { get; init; }
        public string? ExpiryDate { get; init; }
        public CardStatus? Status { get; set; }       
        public Pan Pan { get; set; } = null!;
        public string? PinCode { get; set; }
        public int CountOfPinAttempts { get; set; } = 0;
    }
}
