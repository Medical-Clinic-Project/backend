using backend.clinicalbackend.constants;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto dto
    )
    {
        var result = await authService.RegisterAsync(dto);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto dto
    )
    {
        var result = await authService.LoginAsync(dto);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnAuthorizedException(
                "Refresh token is missing."
            );
        }

        var result = await authService.RefreshAsync(refreshToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await authService.LogoutAsync(refreshToken);
        }

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(
                    JwtTokenConstants.RefreshTokenDays
                ),
                Path = "/api/auth"
            }
        );
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            }
        );
    }
}
