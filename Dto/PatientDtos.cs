namespace backend.clinicalbackend.Dto;

public record UpdatePatientDto(
    string? FullName,
    string? Email,
    bool? IsActive
);

public record PatientResponseDto(
    int Id,
    string FullName,
    string Email,
    bool IsActive
);
