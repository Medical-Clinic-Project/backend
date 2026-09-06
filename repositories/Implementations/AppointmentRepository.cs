using backend.clinicalbackend.Data;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.repositories.Implementations;

public class AppointmentRepository(AppDbContext db)
    : IAppointmentRepository
{
    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await CreateReadQuery()
            .FirstOrDefaultAsync(appointment => appointment.Id == id);
    }

    public async Task<Appointment?> GetByIdForUpdateAsync(int id)
    {
        return await CreateUpdateQuery()
            .FirstOrDefaultAsync(appointment => appointment.Id == id);
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync(
        int? patientId,
        int? doctorId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? from,
        DateTime? to,
        int? departmentId = null,
        string? search = null
    )
    {
        var query = CreateReadQuery();

        if (patientId.HasValue)
        {
            query = query.Where(appointment =>
                appointment.PatientId == patientId.Value
            );
        }

        if (doctorId.HasValue)
        {
            query = query.Where(appointment =>
                appointment.DoctorId == doctorId.Value
            );
        }

        if (statuses is not null)
        {
            var requestedStatuses = statuses.ToArray();

            query = query.Where(appointment =>
                requestedStatuses.Contains(appointment.Status)
            );
        }

        if (from.HasValue)
        {
            query = query.Where(appointment =>
                appointment.EndTime > from.Value
            );
        }

        if (to.HasValue)
        {
            query = query.Where(appointment =>
                appointment.StartTime < to.Value
            );
        }

        if (departmentId.HasValue)
        {
            query = query.Where(appointment =>
                appointment.Doctor.DepartmentId == departmentId.Value
            );
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();

            query = query.Where(appointment =>
                appointment.Patient.FullName.ToLower().Contains(
                    normalizedSearch
                ) ||
                appointment.Patient.Email.ToLower().Contains(
                    normalizedSearch
                ) ||
                appointment.Doctor.User.FullName.ToLower().Contains(
                    normalizedSearch
                ) ||
                appointment.Doctor.Department.Name.ToLower().Contains(
                    normalizedSearch
                )
            );
        }

        return await query
            .OrderBy(appointment => appointment.StartTime)
            .ThenBy(appointment => appointment.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(
        int patientId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? from,
        DateTime? to
    )
    {
        return await GetAllAsync(
            patientId,
            null,
            statuses,
            from,
            to
        );
    }

    public async Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(
        int doctorId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? from,
        DateTime? to
    )
    {
        return await GetAllAsync(
            null,
            doctorId,
            statuses,
            from,
            to
        );
    }

    public async Task<IReadOnlyList<Appointment>>
        GetUpcomingByDoctorIdAsync(
            int doctorId,
            IReadOnlyCollection<AppointmentStatus> statuses,
            DateTime startTime
        )
    {
        ArgumentNullException.ThrowIfNull(statuses);

        var requestedStatuses = statuses.ToArray();

        return await CreateReadQuery()
            .Where(appointment =>
                appointment.DoctorId == doctorId &&
                requestedStatuses.Contains(appointment.Status) &&
                appointment.StartTime >= startTime
            )
            .OrderBy(appointment => appointment.StartTime)
            .ThenBy(appointment => appointment.Id)
            .ToListAsync();
    }

    public async Task<bool> HasDoctorOverlapAsync(
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<AppointmentStatus> blockingStatuses,
        int? excludedAppointmentId = null
    )
    {
        return await GetAppointmentsWithStatusesQuery(
                blockingStatuses,
                excludedAppointmentId
            )
            .AnyAsync(appointment =>
                appointment.DoctorId == doctorId &&
                appointment.StartTime < endTime &&
                appointment.EndTime > startTime
            );
    }

    public async Task<bool> HasPatientOverlapAsync(
        int patientId,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<AppointmentStatus> blockingStatuses,
        int? excludedAppointmentId = null
    )
    {
        return await GetAppointmentsWithStatusesQuery(
                blockingStatuses,
                excludedAppointmentId
            )
            .AnyAsync(appointment =>
                appointment.PatientId == patientId &&
                appointment.StartTime < endTime &&
                appointment.EndTime > startTime
            );
    }

    public async Task<bool> HasDuplicateAppointmentAsync(
        int patientId,
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<AppointmentStatus> blockingStatuses,
        int? excludedAppointmentId = null
    )
    {
        return await GetAppointmentsWithStatusesQuery(
                blockingStatuses,
                excludedAppointmentId
            )
            .AnyAsync(appointment =>
                appointment.PatientId == patientId &&
                appointment.DoctorId == doctorId &&
                appointment.StartTime == startTime &&
                appointment.EndTime == endTime
            );
    }

    public async Task AddAsync(Appointment appointment)
    {
        await db.Appointments.AddAsync(appointment);
    }

    public async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation
    )
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (db.Database.CurrentTransaction is not null)
        {
            return await operation();
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable
            );

        try
        {
            var result = await operation();

            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private IQueryable<Appointment> CreateReadQuery()
    {
        return db.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
                .ThenInclude(doctor => doctor.User)
            .Include(appointment => appointment.Doctor)
                .ThenInclude(doctor => doctor.Department);
    }

    private IQueryable<Appointment> CreateUpdateQuery()
    {
        return db.Appointments
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
                .ThenInclude(doctor => doctor.User)
            .Include(appointment => appointment.Doctor)
                .ThenInclude(doctor => doctor.Department);
    }

    private IQueryable<Appointment> GetAppointmentsWithStatusesQuery(
        IReadOnlyCollection<AppointmentStatus> statuses,
        int? excludedAppointmentId
    )
    {
        ArgumentNullException.ThrowIfNull(statuses);

        var requestedStatuses = statuses.ToArray();

        var query = db.Appointments
            .AsNoTracking()
            .Where(appointment =>
                requestedStatuses.Contains(appointment.Status)
            );

        if (excludedAppointmentId.HasValue)
        {
            query = query.Where(appointment =>
                appointment.Id != excludedAppointmentId.Value
            );
        }

        return query;
    }
}
