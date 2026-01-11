using Application.DTO.TransactionDTO;
using Application.Validators.CustomValidation;
using FluentValidation;

namespace Application.Validators.AccountValidators
{
    public class SavingAccountGenerationValidator : AbstractValidator<SavingsRequestDTO>
    {
        public SavingAccountGenerationValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount cannot be empty.")
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.LastNumbers).LastNumbersRule();

        }
    }
}
