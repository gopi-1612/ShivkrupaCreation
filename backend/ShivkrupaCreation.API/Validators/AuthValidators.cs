using FluentValidation;
using ShivkrupaCreation.API.DTOs.Auth;

namespace ShivkrupaCreation.API.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[6-9]\d{9}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Enter a valid 10-digit mobile number.");

            //RuleFor(x => x.GstNumber)
            //    .Matches(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[A-Z\d]{1}[Z]{1}[A-Z\d]{1}$")
            //    .When(x => !string.IsNullOrEmpty(x.GstNumber))
            //    .WithMessage("Enter a valid GST number.");
        }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
