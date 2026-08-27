namespace backend.clinicalbackend.Dto;

public record CreateDoctorAvailabilityDto(
    DateTime StartTime,
    DateTime EndTime
);

public record UpdateDoctorAvailabilityDto(
    DateTime StartTime,
    DateTime EndTime
);

public record DoctorAvailabilityResponseDto(
    int Id,
    int DoctorId,
    DateTime StartTime,
    DateTime EndTime
);
