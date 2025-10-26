namespace Application.DTO.TransactionDTO
{
    public record TransactionHistoryQueryDTO(
    DateOnly? startDate,
    DateOnly? endDate,
    int? pageNumber,
    int? pageSize
    );
}
