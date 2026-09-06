using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await db.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludedUserId = null
    )
    {
        return await db.Users
            .AnyAsync(user =>
                user.Email == email &&
                (!excludedUserId.HasValue ||
                    user.Id != excludedUserId.Value)
            );
    }

    public async Task AddAsync(User user)
    {
        await db.Users.AddAsync(user);

    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}
