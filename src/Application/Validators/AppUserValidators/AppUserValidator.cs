using Application.DTO.AppUserDTO;
using Application.Validators.CustomValidation;
using FluentValidation;
using System.Data;
namespace Application.Validators.AppUserValidators
{
    public class AppUserValidator : AbstractValidator<AppUserCreateDTO>
    {
        public AppUserValidator()
        {
            RuleFor(user => user.Email).EmailRule();

            RuleFor(user => user.Iin).IINRule();

            RuleFor(user => user.PinCode).PinCodeRule();

            RuleFor(user => user.PasswordHash).PasswordRule();

        }
    }
}
