using Domain.Enums;
namespace Domain.Models
{
    public class Card
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public string? Pan { get; set; }
        public string? PanMasked { get; set; }
        public string? ExpiryDate { get; set; }
        public CardStatus? Status { get; set; }
    }
}
