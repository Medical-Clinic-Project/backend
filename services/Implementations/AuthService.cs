using backend.clinicalbackend.constants.Auth;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;

namespace backend.clinicalbackend.Services.Implementations;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    IValidator<RegisterDto> registerValidator,
    IValidator<LoginDto> loginValidator
) : IAuthService
{
    public async Task<(AuthResponseDto Response, string RefreshToken)>
        RegisterAsync(RegisterDto dto)
    {
        await registerValidator.EnsureValidAsync(dto);

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await userRepository.EmailExistsAsync(email))
        {
            throw new ConflictException(
                AuthMessages.EmailAlreadyExists
            );
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Patient
        };

        await userRepository.AddAsync(user);

        // Must save first so SQL generates user.Id
        await userRepository.SaveChangesAsync();

        var accessToken = jwtService.CreateAccessToken(user);
        var refreshToken = jwtService.CreateRefreshToken(user);

        await refreshTokenRepository.AddAsync(refreshToken);
        await refreshTokenRepository.SaveChangesAsync();

        var response = CreateAuthResponse(user, accessToken);

        return (response, refreshToken.Token);
    }

    public async Task<(AuthResponseDto Response, string RefreshToken)>
        LoginAsync(LoginDto dto)
    {
        await loginValidator.EnsureValidAsync(dto);

        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await userRepository.GetByEmailAsync(email);

        if (
            user is null ||
            !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)
        )
        {
            throw new UnAuthorizedException(
                AuthMessages.InvalidCredentials
            );
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException(
                AuthMessages.AccountInactive
            );
        }

        var accessToken = jwtService.CreateAccessToken(user);
        var refreshToken = jwtService.CreateRefreshToken(user);

        await refreshTokenRepository.AddAsync(refreshToken);
        await refreshTokenRepository.SaveChangesAsync();

        var response = CreateAuthResponse(user, accessToken);

        return (response, refreshToken.Token);
    }

    public async Task<(AuthResponseDto Response, string RefreshToken)>
        RefreshAsync(string refreshToken)
    {
        var storedToken =
            await refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (
            storedToken is null ||
            storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow
        )
        {
            throw new UnAuthorizedException(
                AuthMessages.InvalidOrExpiredRefreshToken
            );
        }

        if (!storedToken.User.IsActive)
        {
            throw new ForbiddenException(
                AuthMessages.AccountInactive
            );
        }

        // Revoke old refresh token
        storedToken.IsRevoked = true;

        // Rotate refresh token
        var newRefreshToken =
            jwtService.CreateRefreshToken(storedToken.User);

        await refreshTokenRepository.AddAsync(newRefreshToken);

        await refreshTokenRepository.SaveChangesAsync();

        var accessToken =
            jwtService.CreateAccessToken(storedToken.User);

        var response = CreateAuthResponse(
            storedToken.User,
            accessToken
        );

        return (response, newRefreshToken.Token);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken =
            await refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken is null)
        {
            return;
        }

        storedToken.IsRevoked = true;

        await refreshTokenRepository.SaveChangesAsync();
    }

    private static AuthResponseDto CreateAuthResponse(
        User user,
        string accessToken
    )
    {
        return new AuthResponseDto(
            accessToken,
            user.FullName,
            user.Email,
            user.Role.ToString()
        );
    }
}
