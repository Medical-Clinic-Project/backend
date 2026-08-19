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
                "An account with this email already exists."
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

        await userRepository.AddRefreshTokenAsync(refreshToken);
        await userRepository.SaveChangesAsync();

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
                "Invalid email or password."
            );
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException(
                "This account is inactive."
            );
        }

        var accessToken = jwtService.CreateAccessToken(user);
        var refreshToken = jwtService.CreateRefreshToken(user);

        await userRepository.AddRefreshTokenAsync(refreshToken);
        await userRepository.SaveChangesAsync();

        var response = CreateAuthResponse(user, accessToken);

        return (response, refreshToken.Token);
    }

    public async Task<(AuthResponseDto Response, string RefreshToken)>
        RefreshAsync(string refreshToken)
    {
        var storedToken =
            await userRepository.GetRefreshTokenAsync(refreshToken);

        if (
            storedToken is null ||
            storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow
        )
        {
            throw new UnAuthorizedException(
                "Invalid or expired refresh token."
            );
        }

        if (!storedToken.User.IsActive)
        {
            throw new ForbiddenException(
                "This account is inactive."
            );
        }

        // Revoke old refresh token
        storedToken.IsRevoked = true;

        // Rotate refresh token
        var newRefreshToken =
            jwtService.CreateRefreshToken(storedToken.User);

        await userRepository.AddRefreshTokenAsync(newRefreshToken);

        await userRepository.SaveChangesAsync();

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
            await userRepository.GetRefreshTokenAsync(refreshToken);

        if (storedToken is null)
        {
            return;
        }

        storedToken.IsRevoked = true;

        await userRepository.SaveChangesAsync();
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
