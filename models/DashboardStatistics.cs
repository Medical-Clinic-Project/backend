namespace backend.clinicalbackend.models;

public sealed record DashboardTotals(
    int TotalPatients,
    int TotalDoctors,
    int TotalDepartments,
    int TotalAppointments
);

public sealed record AppointmentStatusCounts(
    int Pending,
    int Confirmed,
    int Completed,
    int Cancelled
)
{
    public static AppointmentStatusCounts Empty { get; } = new(0, 0, 0, 0);
}

public sealed record DepartmentAppointmentCount(
    int DepartmentId,
    string DepartmentName,
    int AppointmentCount
);

public sealed record AppointmentDailyCount(
    DateTime Day,
    int AppointmentCount
);

public sealed record DoctorAppointmentSummary(
    int TodayAppointments,
    int UpcomingAppointments,
    int CompletedAppointments,
    int CancelledAppointments
)
{
    public static DoctorAppointmentSummary Empty { get; } =
        new(0, 0, 0, 0);
}
