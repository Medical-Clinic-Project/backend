using backend.clinicalbackend.models;

namespace backend.clinicalbackend.repositories.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);

    Task<Appointment?> GetByIdForUpdateAsync(int id);

    Task<IReadOnlyList<Appointment>> GetAllAsync(
        int? patientId,
        int? doctorId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? from,
        DateTime? to
    );

    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(
        int patientId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? from,
        DateTime? to
    );

    Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(
        int doctorId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? from,
        DateTime? to
    );

    Task<IReadOnlyList<Appointment>> GetUpcomingByDoctorIdAsync(
        int doctorId,
        IReadOnlyCollection<AppointmentStatus> statuses,
        DateTime startTime
    );

    Task<bool> HasDoctorOverlapAsync(
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<AppointmentStatus> blockingStatuses,
        int? excludedAppointmentId = null
    );

    Task<bool> HasPatientOverlapAsync(
        int patientId,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<AppointmentStatus> blockingStatuses,
        int? excludedAppointmentId = null
    );

    Task<bool> HasDuplicateAppointmentAsync(
        int patientId,
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        IReadOnlyCollection<AppointmentStatus> blockingStatuses,
        int? excludedAppointmentId = null
    );

    Task AddAsync(Appointment appointment);

    Task SaveChangesAsync();

    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
}
