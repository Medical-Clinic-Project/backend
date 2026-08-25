using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Infrastructure.Interfaces;

namespace backend.clinicalbackend.Infrastructure.Implementations;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor
) : ICurrentUser
{
    public int UserId
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new UnAuthorizedException(
                    "An authenticated user is required."
                );
            }

            var userIdClaim =
                principal.FindFirst(ClaimTypes.NameIdentifier)
                ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);

            if (
                userIdClaim is null ||
                !int.TryParse(userIdClaim.Value, out var userId) ||
                userId <= 0
            )
            {
                throw new UnAuthorizedException(
                    "The authenticated user could not be identified."
                );
            }

            return userId;
        }
    }
}
