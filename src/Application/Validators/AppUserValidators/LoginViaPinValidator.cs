using Application.DTO.AuthDTO;
using FluentValidation;
using Application.Validators.CustomValidation;
namespace Application.Validators.AppUserValidators
{
    public class LoginViaPinValidator : AbstractValidator<LoginViaPinDTO>
    {
        public LoginViaPinValidator()
        {
            RuleFor(x => x.Email).EmailRule();
            RuleFor(x => x.PinCode).PinCodeRule();
        }
    }
}
