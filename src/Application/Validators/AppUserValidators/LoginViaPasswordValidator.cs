using Application.DTO.AuthDTO;
using Application.Validators.CustomValidation;
using FluentValidation;
using System.Data;

namespace Application.Validators.AppUserValidators
{
    public class LoginViaPasswordValidator : AbstractValidator<LoginViaPasswordDTO>
    {
        public LoginViaPasswordValidator()
        {
            RuleFor(x => x.Email).EmailRule();
            RuleFor(x => x.Password).PasswordRule();
        }
    }
}
