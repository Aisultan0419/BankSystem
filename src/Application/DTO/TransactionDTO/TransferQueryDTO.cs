
namespace Application.DTO.TransactionDTO
{
    public record TransferQueryDTO(string Iban, decimal Amount, string LastNumbers);
}
