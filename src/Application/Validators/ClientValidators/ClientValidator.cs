using Application.DTO.ClientDTO;
using Application.Validators.CustomValidation;
using FluentValidation;
namespace Application.Validators.ClientValidators
{
    public class ClientValidator : AbstractValidator<ClientCreateDTO>
    {
        public ClientValidator()
        {
            RuleFor(client => client.Iin).IINRule();
            RuleFor(client => client.FullName)
                .NotNull().WithMessage("Full Name is required.")
                .Matches("^[a-zA-Zа-яА-ЯёЁ\\s'-]+$").WithMessage("Full Name can only contain letters, spaces, hyphens, and apostrophes.");
            RuleFor(client => client.PhoneNumber)
                .NotEmpty().WithMessage("Phone Number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone Number must be in a valid format.");
        }
    }
}
