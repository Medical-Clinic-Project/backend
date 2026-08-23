using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IDoctorService
{
    Task<IReadOnlyList<Doctor>> GetAllAsync(
        string? search,
        int? departmentId
    );

    Task<Doctor> GetByIdAsync(int id);

    Task<Doctor> CreateAsync(CreateDoctorDto dto);

    Task<Doctor> UpdateAsync(int id, UpdateDoctorDto dto);

    Task<Doctor> UpdateStatusAsync(
        int id,
        UpdateDoctorStatusDto dto
    );
}
