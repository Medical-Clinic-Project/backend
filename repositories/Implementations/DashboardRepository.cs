using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Policies.Appointments;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class DashboardRepository(AppDbContext db) : IDashboardRepository
{
    public async Task<DashboardTotals> GetAdminTotalsAsync()
    {
        var totalPatients = await db.Users
            .AsNoTracking()
            .CountAsync(user => user.Role == UserRole.Patient);

        var totalDoctors = await db.Doctors
            .AsNoTracking()
            .CountAsync();

        var totalDepartments = await db.Departments
            .AsNoTracking()
            .CountAsync();

        var totalAppointments = await db.Appointments
            .AsNoTracking()
            .CountAsync();

        return new DashboardTotals(
            totalPatients,
            totalDoctors,
            totalDepartments,
            totalAppointments
        );
    }

    public async Task<AppointmentStatusCounts> GetAppointmentStatusCountsAsync(
        int? doctorId = null
    )
    {
        var query = CreateAppointmentQuery(doctorId);

        return await query
            .GroupBy(_ => 1)
            .Select(group => new AppointmentStatusCounts(
                group.Count(appointment =>
                    appointment.Status == AppointmentStatus.Pending
                ),
                group.Count(appointment =>
                    appointment.Status == AppointmentStatus.Confirmed
                ),
                group.Count(appointment =>
                    appointment.Status == AppointmentStatus.Completed
                ),
                group.Count(appointment =>
                    appointment.Status == AppointmentStatus.Cancelled
                )
            ))
            .SingleOrDefaultAsync() ?? AppointmentStatusCounts.Empty;
    }

    public async Task<IReadOnlyList<DepartmentAppointmentCount>>
        GetAppointmentCountsByDepartmentAsync()
    {
        return await db.Appointments
            .AsNoTracking()
            .GroupBy(appointment => new
            {
                appointment.Doctor.DepartmentId,
                appointment.Doctor.Department.Name
            })
            .OrderBy(group => group.Key.Name)
            .ThenBy(group => group.Key.DepartmentId)
            .Select(group => new DepartmentAppointmentCount(
                group.Key.DepartmentId,
                group.Key.Name,
                group.Count()
            ))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AppointmentDailyCount>>
        GetAppointmentCountsByDayAsync(
            DateTime from,
            DateTime to,
            int? doctorId = null
        )
    {
        return await CreateAppointmentQuery(doctorId)
            .Where(appointment =>
                appointment.StartTime >= from &&
                appointment.StartTime < to
            )
            .GroupBy(appointment => appointment.StartTime.Date)
            .OrderBy(group => group.Key)
            .Select(group => new AppointmentDailyCount(
                group.Key,
                group.Count()
            ))
            .ToListAsync();
    }

    public async Task<DoctorAppointmentSummary>
        GetDoctorAppointmentSummaryAsync(
            int doctorId,
            DateTime utcNow
        )
    {
        var dayStart = utcNow.Date;
        var dayEnd = dayStart.AddDays(1);
        var blockingStatuses =
            AppointmentStatusPolicy.BlockingStatuses.ToArray();

        return await CreateAppointmentQuery(doctorId)
            .GroupBy(_ => 1)
            .Select(group => new DoctorAppointmentSummary(
                group.Count(appointment =>
                    appointment.EndTime > dayStart &&
                    appointment.StartTime < dayEnd
                ),
                group.Count(appointment =>
                    blockingStatuses.Contains(appointment.Status) &&
                    appointment.StartTime >= utcNow
                ),
                group.Count(appointment =>
                    appointment.Status == AppointmentStatus.Completed
                ),
                group.Count(appointment =>
                    appointment.Status == AppointmentStatus.Cancelled
                )
            ))
            .SingleOrDefaultAsync() ?? DoctorAppointmentSummary.Empty;
    }

    private IQueryable<Appointment> CreateAppointmentQuery(int? doctorId)
    {
        var query = db.Appointments.AsNoTracking();

        if (doctorId.HasValue)
        {
            query = query.Where(appointment =>
                appointment.DoctorId == doctorId.Value
            );
        }

        return query;
    }
}
