
namespace Application.DTO.TransactionDTO
{
    public class TransferQueryDTO
    {
        public required string Iban { get; set; }
        public required decimal Amount { get; set; }
        public required string LastNumbers { get; set; }
    }
}
