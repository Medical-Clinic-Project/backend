using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class DoctorRepository(AppDbContext db) : IDoctorRepository
{
    public async Task<IReadOnlyList<Doctor>> GetAllAsync(
        string? search,
        int? departmentId
    )
    {
        IQueryable<Doctor> query = db.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.User)
            .Include(doctor => doctor.Department);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();

            query = query.Where(doctor =>
                doctor.User.FullName.ToLower().Contains(normalizedSearch) ||
                doctor.User.Email.ToLower().Contains(normalizedSearch) ||
                doctor.Department.Name.ToLower().Contains(normalizedSearch)
            );
        }

        if (departmentId.HasValue)
        {
            query = query.Where(doctor =>
                doctor.DepartmentId == departmentId.Value
            );
        }

        return await query
            .OrderBy(doctor => doctor.User.FullName)
            .ThenBy(doctor => doctor.Id)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await db.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.User)
            .Include(doctor => doctor.Department)
            .FirstOrDefaultAsync(doctor => doctor.Id == id);
    }

    public async Task<Doctor?> GetByIdForUpdateAsync(int id)
    {
        return await db.Doctors
            .Include(doctor => doctor.User)
            .Include(doctor => doctor.Department)
            .FirstOrDefaultAsync(doctor => doctor.Id == id);
    }

    public async Task<Doctor?> GetByUserIdAsync(int userId)
    {
        return await db.Doctors
            .AsNoTracking()
            .Include(doctor => doctor.User)
            .FirstOrDefaultAsync(doctor => doctor.UserId == userId);
    }

    public async Task AddAsync(Doctor doctor)
    {
        await db.Doctors.AddAsync(doctor);
    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}
