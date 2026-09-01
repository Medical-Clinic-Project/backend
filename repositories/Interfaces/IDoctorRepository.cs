using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IDoctorRepository
{
    Task<IReadOnlyList<Doctor>> GetAllAsync(
        string? search,
        int? departmentId
    );

    Task<Doctor?> GetByIdAsync(int id);

    Task<IReadOnlyList<Doctor>> GetActiveForPatientsAsync(
        string? search,
        int? departmentId
    );

    Task<Doctor?> GetActiveForPatientByIdAsync(int id);

    Task<Doctor?> GetByIdForUpdateAsync(int id);

    Task<Doctor?> GetByUserIdAsync(int userId);

    Task AddAsync(Doctor doctor);

    Task SaveChangesAsync();
}
