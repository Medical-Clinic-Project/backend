using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Dto;

public record CreateAppointmentDto(
    int DoctorId,
    DateTime StartTime,
    DateTime EndTime,
    string? Reason,
    string? Notes
);

public record RescheduleAppointmentDto(
    DateTime StartTime,
    DateTime EndTime
);

public record UpdateAppointmentStatusDto(
    AppointmentStatus Status
);

public record AppointmentFilterDto(
    DateTime? From,
    DateTime? To,
    int? DoctorId,
    int? PatientId,
    AppointmentStatus? Status
);

public record AppointmentResponseDto(
    int Id,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    int DepartmentId,
    string DepartmentName,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status,
    string? Reason,
    string? Notes
);
