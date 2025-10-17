using FluentValidation;
using Application.DTO.TransactionDTO;
namespace Application.Validators.TransactionQueryValidators
{
    public class TransactionGetHistoryValidator : AbstractValidator<TransactionHistoryQueryDTO>
    {
        public TransactionGetHistoryValidator()
        {
            RuleFor(x => x.pageNumber)
                .GreaterThan(0).When(x => x.pageNumber.HasValue)
                .WithMessage("pageNumber must be greater than 0.");
            RuleFor(x => x.pageSize)
                .GreaterThan(0).When(x => x.pageSize.HasValue)
                .WithMessage("pageSize must be greater than 0.");
            RuleFor(x => x.startDate)
                .InclusiveBetween(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)), DateOnly.FromDateTime(DateTime.Now))
                .When(x => x.startDate.HasValue)
                .WithMessage($"Start date can be only between {DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))} and {DateOnly.FromDateTime(DateTime.Now)}");
            RuleFor(x => x.endDate)
                .InclusiveBetween(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)), DateOnly.FromDateTime(DateTime.Now))
                .When(x => x.endDate.HasValue)
                .WithMessage($"End date can be only between {DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))} and {DateOnly.FromDateTime(DateTime.Now)}");
        }
    }
}
