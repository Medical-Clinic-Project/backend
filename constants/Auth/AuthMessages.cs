namespace backend.clinicalbackend.constants.Auth;

public static class AuthMessages
{
    public const string EmailAlreadyExists =
        "An account with this email already exists.";

    public const string InvalidCredentials =
        "Invalid email or password.";

    public const string AccountInactive =
        "This account is inactive.";

    public const string InvalidOrExpiredRefreshToken =
        "Invalid or expired refresh token.";

    public const string RefreshTokenMissing =
        "Refresh token is missing.";
}
