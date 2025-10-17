namespace Application.DTO.TransactionDTO
{
    public class TransactionHistoryQueryDTO
    {
        public DateOnly? startDate { get;  set; }
        public DateOnly? endDate { get;  set; }
        public int? pageNumber  { get; set; }
        public int? pageSize { get; set; }
    }
}
