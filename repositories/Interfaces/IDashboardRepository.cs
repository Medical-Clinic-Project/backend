using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardTotals> GetAdminTotalsAsync();

    Task<AppointmentStatusCounts> GetAppointmentStatusCountsAsync(
        int? doctorId = null
    );

    Task<IReadOnlyList<DepartmentAppointmentCount>>
        GetAppointmentCountsByDepartmentAsync();

    Task<IReadOnlyList<AppointmentDailyCount>>
        GetAppointmentCountsByDayAsync(
            DateTime from,
            DateTime to,
            int? doctorId = null
        );

    Task<DoctorAppointmentSummary> GetDoctorAppointmentSummaryAsync(
        int doctorId,
        DateTime utcNow
    );
}
