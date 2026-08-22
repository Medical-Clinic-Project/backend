using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class DepartmentRepository(AppDbContext db)

    : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetAllAsync(
        string? search
    )
    {
        var query = db.Departments.AsNoTracking();


        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();

            query = query.Where(department =>
                department.Name.ToLower().Contains(normalizedSearch) ||
                department.Description.ToLower().Contains(normalizedSearch)
            );
        }

        return await query
            .OrderBy(department => department.Name)
            .ThenBy(department => department.Id)
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {

        return await db.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(department => department.Id == id);
    }

    public async Task<Department?> GetByIdForUpdateAsync(int id)
    {
        return await db.Departments
            .FirstOrDefaultAsync(department => department.Id == id);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        int? excludedDepartmentId = null
    )
    {
        var normalizedName = name.ToLowerInvariant();


        return await db.Departments.AnyAsync(department =>
            department.Name.ToLower() == normalizedName &&
            (!excludedDepartmentId.HasValue ||
                department.Id != excludedDepartmentId.Value)
        );
    }

    public async Task AddAsync(Department department)
    {
        await db.Departments.AddAsync(department);

    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }
}
