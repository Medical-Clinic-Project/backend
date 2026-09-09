using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class DoctorAvailabilityRepository(AppDbContext db)
    : IDoctorAvailabilityRepository
{
    public async Task<IReadOnlyList<DoctorAvailability>>
        GetByDoctorIdAsync(
            int doctorId,
            DateTime? from,
            DateTime? to
        )
    {
        var query = db.DoctorAvailabilities
            .AsNoTracking()
            .Where(availability => availability.DoctorId == doctorId);

        if (from.HasValue)
        {
            query = query.Where(availability =>
                availability.EndTime > from.Value
            );
        }

        if (to.HasValue)
        {
            query = query.Where(availability =>
                availability.StartTime < to.Value
            );
        }

        return await query
            .OrderBy(availability => availability.StartTime)
            .ThenBy(availability => availability.Id)
            .ToListAsync();
    }

    public async Task<DoctorAvailability?> GetByIdForUpdateAsync(
        int id
    )
    {
        return await db.DoctorAvailabilities
            .FirstOrDefaultAsync(availability =>
                availability.Id == id
            );
    }

    public async Task<bool> HasOverlapAsync(
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        int? excludedAvailabilityId = null
    )
    {
        return await db.DoctorAvailabilities.AnyAsync(availability =>
            availability.DoctorId == doctorId &&
            availability.StartTime < endTime &&
            availability.EndTime > startTime &&
            (!excludedAvailabilityId.HasValue ||
                availability.Id != excludedAvailabilityId.Value)
        );
    }

    public async Task AddAsync(DoctorAvailability availability)
    {
        await db.DoctorAvailabilities.AddAsync(availability);
    }

    public void Delete(DoctorAvailability availability)
    {
        db.DoctorAvailabilities.Remove(availability);
    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}
