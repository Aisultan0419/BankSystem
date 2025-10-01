using Application.DTO.TransactionDTO;
using FluentValidation;
using Application.Validators.CustomValidation;
namespace Application.Validators.TransactionQueryValidators
{
    public class DepositQueryValidator : AbstractValidator<DepositQueryDTO>
    {
        public DepositQueryValidator()
        {
            RuleFor(x => x.Amount).AmountRule();
            RuleFor(x => x.LastNumbers).LastNumbersRule();
        }
    }
}
