using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IPatientRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(
        string? search,
        bool? isActive
    );

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByIdForUpdateAsync(int id);

    Task SaveChangesAsync();
}
