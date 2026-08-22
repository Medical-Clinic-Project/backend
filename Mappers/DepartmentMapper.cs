using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Mappers;

public static class DepartmentMapper
{
    public static DepartmentResponseDto ToResponseDto(
        this Department department
    )
    {
        return new DepartmentResponseDto(
            department.Id,
            department.Name,
            department.Description,
            department.IsActive
        );
    }
}
