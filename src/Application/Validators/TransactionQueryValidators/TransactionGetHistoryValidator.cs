using FluentValidation;
using Application.DTO.TransactionDTO;
namespace Application.Validators.TransactionQueryValidators
{
    public class TransactionGetHistoryValidator : AbstractValidator<TransactionHistoryQueryDTO>
    {
        public TransactionGetHistoryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).When(x => x.PageNumber.HasValue)
                .WithMessage("pageNumber must be greater than 0.");
            RuleFor(x => x.PageSize)
                .GreaterThan(0).When(x => x.PageSize.HasValue)
                .WithMessage("pageSize must be greater than 0.");
            RuleFor(x => x.StartDate)
                .InclusiveBetween(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)), DateOnly.FromDateTime(DateTime.Now))
                .When(x => x.StartDate.HasValue)
                .WithMessage($"Start date can be only between {DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))} and {DateOnly.FromDateTime(DateTime.Now)}");
            RuleFor(x => x.EndDate)
                .InclusiveBetween(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)), DateOnly.FromDateTime(DateTime.Now))
                .When(x => x.EndDate.HasValue)
                .WithMessage($"End date can be only between {DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))} and {DateOnly.FromDateTime(DateTime.Now)}");
        }
    }
}
