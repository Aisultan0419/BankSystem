namespace Application.DTO.TransactionDTO
{
    public record TransactionHistoryQueryDTO(
    DateOnly? StartDate,
    DateOnly? EndDate,
    int? PageNumber,
    int? PageSize
    );
}
