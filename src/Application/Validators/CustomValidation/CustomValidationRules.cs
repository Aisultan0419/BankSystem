using FluentValidation;
namespace Application.Validators.CustomValidation
{
    public static class CustomValidationRules
    {
        public static IRuleBuilderOptions<T, string> EmailRule<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
        public static IRuleBuilderOptions<T, string> IINRule<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("IIN is required.")
                .Length(12).WithMessage("IIN must be exactly 12 characters long.")
                .Matches("^[0-9]{12}$").WithMessage("IIN must contain only digits.");
        }
        public static IRuleBuilderOptions<T, string> PasswordRule<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
        public static IRuleBuilderOptions<T, string> PinCodeRule<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("PinCode is required.")
                .Length(4).WithMessage("PinCode must be exactly 4 characters long.")
                .Matches("^[0-9]{4}$").WithMessage("PinCode must contain only digits.");
        }
        public static IRuleBuilderOptions<T, string> LastNumbersRule<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Last numbers should not be empty")
                .Length(4).WithMessage("Last numbers should be exactly 4 digits")
                .Matches("^[0-9]{4}$").WithMessage("Last numbers should contain only digits");
        }
        public static IRuleBuilderOptions<T, decimal> AmountRule<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Amount is empty")
                .GreaterThan(0).WithMessage("Amount must be greater than 0")
                .LessThan(2000000m).WithMessage("Amount must be less than 2 000 000");
        }
        public static IRuleBuilderOptions<T, string> IbanRule<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("IBAN is required.")
                .Length(21).WithMessage("IBAN must be exactly 21 characters long.")
                .Matches(@"^KZ[0-9]{2}[A-Z]{4}[0-9]{13}$").WithMessage("IBAN must start with 'KZ', then 2 digits, 4 letters (bank code), and 13 digits.");
        }
    }
}
