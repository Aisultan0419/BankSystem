using Application.DTO.AccountDTO;
using FluentValidation;
namespace Application.Validators.AccountValidators
{
    public class AccountGenerationValidator : AbstractValidator<AccountCreateQueryDTO>
    {
        public AccountGenerationValidator()
        {
            RuleFor(x => x.AccountType).IsInEnum().WithMessage("Invalid account type.");
        }
    }
}
