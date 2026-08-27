using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IDoctorAvailabilityService
{
    Task<IReadOnlyList<DoctorAvailability>> GetMineAsync(
        DateTime? from,
        DateTime? to
    );

    Task<IReadOnlyList<DoctorAvailability>> GetByDoctorAsync(
        int doctorId,
        DateTime? from,
        DateTime? to
    );

    Task<DoctorAvailability> CreateAsync(
        CreateDoctorAvailabilityDto dto
    );

    Task<DoctorAvailability> UpdateAsync(
        int id,
        UpdateDoctorAvailabilityDto dto
    );

    Task DeleteAsync(int id);
}
