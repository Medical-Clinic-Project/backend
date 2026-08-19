using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.clinicalbackend.constants;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace backend.clinicalbackend.Services.Implementations;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string CreateAccessToken(User user)
    {
        var secretKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is missing.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is missing.");

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()
            ),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                JwtTokenConstants.AccessTokenMinutes
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(User user)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)
            ),

            ExpiresAt = DateTime.UtcNow.AddDays(
                JwtTokenConstants.RefreshTokenDays
            ),

            IsRevoked = false,

            UserId = user.Id
        };
    }
}
