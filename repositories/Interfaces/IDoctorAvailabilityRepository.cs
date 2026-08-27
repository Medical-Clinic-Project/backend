using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IDoctorAvailabilityRepository
{
    Task<IReadOnlyList<DoctorAvailability>> GetByDoctorIdAsync(
        int doctorId,
        DateTime? from,
        DateTime? to
    );

    Task<DoctorAvailability?> GetByIdForUpdateAsync(int id);

    Task<bool> HasOverlapAsync(
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        int? excludedAvailabilityId = null
    );

    Task AddAsync(DoctorAvailability availability);

    void Delete(DoctorAvailability availability);

    Task SaveChangesAsync();
}
