using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Dto;

public record AdminDashboardTotalsDto(
    int TotalPatients,
    int TotalDoctors,
    int TotalDepartments,
    int TotalAppointments
);

public record AppointmentStatusCountDto(
    AppointmentStatus Status,
    int Count
);

public record DepartmentAppointmentCountDto(
    int DepartmentId,
    string DepartmentName,
    int AppointmentCount
);

public record AppointmentTimeSeriesPointDto(
    DateTime Date,
    int AppointmentCount
);

public record AdminDashboardResponseDto(
    AdminDashboardTotalsDto Totals,
    IReadOnlyList<AppointmentStatusCountDto> AppointmentsByStatus,
    IReadOnlyList<DepartmentAppointmentCountDto>
        AppointmentsByDepartment,
    IReadOnlyList<AppointmentTimeSeriesPointDto>
        AppointmentsOverTime
);

public record DoctorDashboardSummaryDto(
    int TodayAppointments,
    int UpcomingAppointments,
    int CompletedAppointments,
    int CancelledAppointments
);

public record DoctorDashboardResponseDto(
    DoctorDashboardSummaryDto Summary,
    IReadOnlyList<AppointmentStatusCountDto> AppointmentsByStatus,
    IReadOnlyList<AppointmentTimeSeriesPointDto> AppointmentsByDay,
    IReadOnlyList<AppointmentTimeSeriesPointDto>
        AppointmentsOverTime
);
