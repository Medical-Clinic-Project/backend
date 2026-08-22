namespace backend.clinicalbackend.Dto;

public record CreateDepartmentDto(
    string Name,
    string Description,
    bool IsActive
);

public record UpdateDepartmentDto(
    string Name,
    string Description,
    bool IsActive
);

public record UpdateDepartmentStatusDto(
    bool IsActive
);

public record DepartmentResponseDto(
    int Id,
    string Name,
    string Description,
    bool IsActive
);
