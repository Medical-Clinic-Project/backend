using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class RefreshTokenRepository(AppDbContext db)
    : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await db.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken =>
                refreshToken.Token == token
            );
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await db.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}
