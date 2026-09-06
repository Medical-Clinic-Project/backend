using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IPatientService
{
    Task<IReadOnlyList<User>> GetAllAsync(
        string? search,
        bool? isActive
    );

    Task<User> GetByIdAsync(int id);

    Task<User> GetCurrentPatientAsync();

    Task<User> UpdateAsync(
        int id,
        UpdatePatientDto dto
    );
}
