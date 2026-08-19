using FluentValidation.Results;
using System.Text.Json;

namespace backend.clinicalbackend.exceptions;

public class ValidationsException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationsException(IEnumerable<ValidationFailure> failures)
        : base("Validation failed.")
    {
        Errors = failures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => JsonNamingPolicy.CamelCase.ConvertName(group.Key),
                group => group
                    .Select(failure => failure.ErrorMessage)
                    .ToArray()
            );
    }
}

public class NotFoundException(string message)
    : Exception(message);

public class ConflictException(string message)
    : Exception(message);

public class UnAuthorizedException(string message)
    : Exception(message);

public class ForbiddenException(string message)
    : Exception(message);
