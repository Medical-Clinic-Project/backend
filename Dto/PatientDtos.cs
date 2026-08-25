namespace backend.clinicalbackend.Dto;

public record UpdatePatientProfileDto(
    string FullName,
    string Email
);

public record UpdatePatientStatusDto(
    bool? IsActive
);

public record PatientResponseDto(
    int Id,
    string FullName,
    string Email,
    bool IsActive
);
