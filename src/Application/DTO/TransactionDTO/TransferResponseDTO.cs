
namespace Application.DTO.TransactionDTO
{
    public class TransferResponseDTO { 
        public string? FullName { get; set; } 
        public decimal? TransferredAmount { get; set; } 
        public decimal? RemainingBalance { get; set; } 
        public string? Message { get; set; } 
    }
}
