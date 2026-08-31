using backend.clinicalbackend.Dto;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IAuthService
{
    Task<(AuthResponseDto Response, string RefreshToken)> RegisterAsync(
        RegisterDto dto
    );

    Task<(AuthResponseDto Response, string RefreshToken)> LoginAsync(
        LoginDto dto
    );

    Task<(AuthResponseDto Response, string RefreshToken)> RefreshAsync(
        string refreshToken
    );

    Task LogoutAsync(string refreshToken);
}
