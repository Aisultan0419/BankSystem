using FluentValidation;
using Application.Validators.CustomValidation;
using Application.DTO.ClientDTO;
namespace Application.Validators.ClientValidators
{
    public class IINValidator : AbstractValidator<IINDTO>
    {
        public IINValidator()
        {
            RuleFor(c => c.IIN).IINRule();
        }
    }
}
