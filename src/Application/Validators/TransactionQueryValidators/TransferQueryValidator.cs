using Application.DTO.TransactionDTO;
using FluentValidation;
using Application.Validators.CustomValidation;
namespace Application.Validators.TransactionQueryValidators
{
    public class TransferQueryValidator : AbstractValidator<TransferQueryDTO>
    {
        public TransferQueryValidator()
        {
            RuleFor(a => a.Amount).AmountRule();    
            RuleFor(a => a.LastNumbers).LastNumbersRule();
            RuleFor(a => a.Iban).IbanRule();
        }
    }
}
