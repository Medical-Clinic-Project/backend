using Microsoft.AspNetCore.Diagnostics;


namespace backend.clinicalbackend.exceptions;

public class AppExceptionHandler(
    ILogger<AppExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(
            exception,
            "Unhandled exception occurred."
        );

        int statusCode;
        object response;

        switch (exception)
        {
            case ValidationsException validationException:
                statusCode = StatusCodes.Status400BadRequest;

                response = new
                {
                    message = validationException.Message,
                    errors = validationException.Errors
                };

                break;

            case NotFoundException:
                statusCode = StatusCodes.Status404NotFound;

                response = new
                {
                    message = exception.Message
                };

                break;

            case ConflictException:
                statusCode = StatusCodes.Status409Conflict;

                response = new
                {
                    message = exception.Message
                };

                break;

            case UnAuthorizedException:
                statusCode = StatusCodes.Status401Unauthorized;

                response = new
                {
                    message = exception.Message
                };

                break;

            case ForbiddenException:
                statusCode = StatusCodes.Status403Forbidden;

                response = new
                {
                    message = exception.Message
                };

                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;

                response = new
                {
                    message = "An unexpected error occurred."
                };

                break;
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken
        );

        return true;
    }
}
