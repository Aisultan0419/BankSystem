using Application.DTO;
namespace Application.Interfaces.Services
{
    public interface ITransferService
    {
        Task<TransferResponseDTO> TransferAsync(string appUserId, string iban, decimal amount, string lastNumbers);
    }
}
