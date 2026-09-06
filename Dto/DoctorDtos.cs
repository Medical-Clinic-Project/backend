namespace backend.clinicalbackend.Dto;

public record CreateDoctorDto(
    string FullName,
    string Email,
    string Password,
    int DepartmentId
);

public record UpdateDoctorDto(
    string FullName,
    string Email,
    int DepartmentId,
    bool? IsActive
);

public record DoctorResponseDto(
    int Id,
    string FullName,
    string Email,
    int DepartmentId,
    DepartmentResponseDto Department,
    bool IsActive
);
