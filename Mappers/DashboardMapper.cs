using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Helpers.Appointments;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Mappers;

public static class DashboardMapper
{
    public static AdminDashboardTotalsDto ToDto(
        this DashboardTotals totals
    )
    {
        return new AdminDashboardTotalsDto(
            totals.TotalPatients,
            totals.TotalDoctors,
            totals.TotalDepartments,
            totals.TotalAppointments
        );
    }

    public static IReadOnlyList<AppointmentStatusCountDto> ToDtos(
        this AppointmentStatusCounts counts
    )
    {
        return new[]
        {
            new AppointmentStatusCountDto(
                AppointmentStatus.Pending,
                counts.Pending
            ),
            new AppointmentStatusCountDto(
                AppointmentStatus.Confirmed,
                counts.Confirmed
            ),
            new AppointmentStatusCountDto(
                AppointmentStatus.Completed,
                counts.Completed
            ),
            new AppointmentStatusCountDto(
                AppointmentStatus.Cancelled,
                counts.Cancelled
            )
        };
    }

    public static DepartmentAppointmentCountDto ToDto(
        this DepartmentAppointmentCount count
    )
    {
        return new DepartmentAppointmentCountDto(
            count.DepartmentId,
            count.DepartmentName,
            count.AppointmentCount
        );
    }

    public static AppointmentTimeSeriesPointDto ToDto(
        this AppointmentDailyCount count
    )
    {
        return new AppointmentTimeSeriesPointDto(
            AppointmentTimeHelper.NormalizeUtc(count.Day),
            count.AppointmentCount
        );
    }

    public static DoctorDashboardSummaryDto ToDto(
        this DoctorAppointmentSummary summary
    )
    {
        return new DoctorDashboardSummaryDto(
            summary.TodayAppointments,
            summary.UpcomingAppointments,
            summary.CompletedAppointments,
            summary.CancelledAppointments
        );
    }
}
