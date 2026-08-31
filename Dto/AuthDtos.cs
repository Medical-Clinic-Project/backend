namespace backend.clinicalbackend.Dto;

public record RegisterDto(
    string FullName,
    string Email,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string AccessToken,
    string FullName,
    string Email,
    string Role
);

public record RefreshTokenDto(
    string RefreshToken
);
