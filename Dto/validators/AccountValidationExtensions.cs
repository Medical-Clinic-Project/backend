using System.Linq.Expressions;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators;

public static class AccountValidationExtensions
{
    public static void AddFullNameRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> propertyExpression
    )
    {
        validator.RuleFor(propertyExpression)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MinimumLength(2)
            .WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters.");
    }

    public static void AddEmailRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> propertyExpression
    )
    {
        validator.RuleFor(propertyExpression)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(150)
            .WithMessage("Email cannot exceed 150 characters.");
    }

    public static void AddPasswordRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> propertyExpression
    )
    {
        validator.RuleFor(propertyExpression)
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
