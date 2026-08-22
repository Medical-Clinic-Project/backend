using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(string? search);

    Task<Department?> GetByIdAsync(int id);

    Task<Department?> GetByIdForUpdateAsync(int id);

    Task<bool> NameExistsAsync(
        string name,
        int? excludedDepartmentId = null
    );

    Task AddAsync(Department department);

    Task SaveChangesAsync();
}
