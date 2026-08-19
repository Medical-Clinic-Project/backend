using backend.clinicalbackend.exceptions;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators;

public static class ValidationExtensions
{
    public static async Task EnsureValidAsync<T>(
        this IValidator<T> validator,
        T instance
    )
    {
        var result = await validator.ValidateAsync(instance);

        if (!result.IsValid)
        {
            throw new ValidationsException(result.Errors);
        }
    }
}
