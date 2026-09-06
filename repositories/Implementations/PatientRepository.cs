using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class PatientRepository(AppDbContext db)
    : IPatientRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync(
        string? search,
        bool? isActive
    )
    {
        var query = db.Users
            .AsNoTracking()
            .Where(user => user.Role == UserRole.Patient);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();

            query = query.Where(user =>
                user.FullName.ToLower().Contains(normalizedSearch) ||
                user.Email.ToLower().Contains(normalizedSearch)
            );
        }

        if (isActive.HasValue)
        {
            query = query.Where(user =>
                user.IsActive == isActive.Value
            );
        }

        return await query
            .OrderBy(user => user.FullName)
            .ThenBy(user => user.Id)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user =>
                user.Id == id &&
                user.Role == UserRole.Patient
            );
    }

    public async Task<User?> GetByIdForUpdateAsync(int id)
    {
        return await db.Users
            .FirstOrDefaultAsync(user =>
                user.Id == id &&
                user.Role == UserRole.Patient
            );
    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}
