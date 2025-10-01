using Application.DTO.CardDTO;
using Application.Validators.CustomValidation;
using FluentValidation;
namespace Application.Validators.CardValidators
{
    public class LastNumberValidator : AbstractValidator<LastNumbersDTO>
    {
        public LastNumberValidator()
        {
            RuleFor(x => x.LastNumbers).LastNumbersRule();
        }
    }
}
