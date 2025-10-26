using System.Net;
using Domain.Models;
namespace Application.DTO.TransactionDTO
{
    public class PurchaseResponseDTO {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentType { get; set; } 
    }
}
