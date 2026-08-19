using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.AuthValidators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MinimumLength(2)
            .WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(150)
            .WithMessage("Email cannot exceed 150 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must contain at least 8 characters.")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character.");
    }
}
