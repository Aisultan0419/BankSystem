using FluentValidation;
using Application.Validators.CustomValidation;
using Application.DTO.ClientDTO;
namespace Application.Validators.ClientValidators
{
    public class IINValidator : AbstractValidator<IinDTO>
    {
        public IINValidator()
        {
            RuleFor(c => c.Iin).IINRule();
        }
    }
}
