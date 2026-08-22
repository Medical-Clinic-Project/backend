using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<Department>> GetAllAsync(string? search);

    Task<Department> GetByIdAsync(int id);

    Task<Department> CreateAsync(CreateDepartmentDto dto);

    Task<Department> UpdateAsync(int id, UpdateDepartmentDto dto);

    Task<Department> UpdateStatusAsync(
        int id,
        UpdateDepartmentStatusDto dto
    );
}
