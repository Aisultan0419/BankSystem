
namespace Application.DTO.TransactionDTO
{
    public class DepositQueryDTO
    {
        public decimal Amount { get; set; }
        public string LastNumbers { get; set; } = null!;
    }
}
