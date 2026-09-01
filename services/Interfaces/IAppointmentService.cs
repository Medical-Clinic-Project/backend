using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IAppointmentService
{
    Task<Appointment> CreateAsync(CreateAppointmentDto dto);

    Task<Appointment> GetByIdAsync(int id);

    Task<IReadOnlyList<Appointment>> GetAllAsync(
        AppointmentFilterDto filter
    );

    Task<IReadOnlyList<Appointment>> GetMineAsync(
        AppointmentFilterDto filter
    );

    Task<IReadOnlyList<Appointment>> GetTodayAsync();

    Task<IReadOnlyList<Appointment>> GetUpcomingAsync();

    Task<IReadOnlyList<Appointment>> GetCompletedAsync();

    Task<IReadOnlyList<Appointment>> GetCancelledAsync();

    Task<Appointment> CancelAsync(int id);

    Task<Appointment> RescheduleAsync(
        int id,
        RescheduleAppointmentDto dto
    );

    Task<Appointment> UpdateStatusAsync(
        int id,
        UpdateAppointmentStatusDto dto
    );
}
