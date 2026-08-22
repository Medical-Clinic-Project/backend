using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class DepartmentRepository(AppDbContext context)
    : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetAllAsync(
        string? search
    )
    {
        var query = context.Departments.AsNoTracking();

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
        return await context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(department => department.Id == id);
    }

    public async Task<Department?> GetByIdForUpdateAsync(int id)
    {
        return await context.Departments
            .FirstOrDefaultAsync(department => department.Id == id);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        int? excludedDepartmentId = null
    )
    {
        var normalizedName = name.ToLowerInvariant();

        return await context.Departments.AnyAsync(department =>
            department.Name.ToLower() == normalizedName &&
            (!excludedDepartmentId.HasValue ||
                department.Id != excludedDepartmentId.Value)
        );
    }

    public async Task AddAsync(Department department)
    {
        await context.Departments.AddAsync(department);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
