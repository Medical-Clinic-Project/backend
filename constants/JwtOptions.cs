namespace backend.clinicalbackend.constants;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; }
    public int RefreshTokenDays { get; set; }
}
