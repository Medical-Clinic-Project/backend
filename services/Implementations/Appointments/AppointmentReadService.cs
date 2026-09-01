using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Helpers.Appointments;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Policies.Appointments;
using backend.clinicalbackend.repositories.Interfaces;
using FluentValidation;

namespace backend.clinicalbackend.Services.Implementations.Appointments;

public sealed class AppointmentReadService(
    IAppointmentRepository appointmentRepository,
    AppointmentAuthorizationService authorizationService,
    IValidator<AppointmentFilterDto> filterValidator
)
{
    public async Task<Appointment> GetByIdAsync(int id)
    {
        var appointment = await GetAppointmentAsync(id);
        await authorizationService.EnsureCanViewAsync(appointment);

        return appointment;
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync(
        AppointmentFilterDto filter
    )
    {
        var normalizedFilter = await NormalizeFilterAsync(filter);
        await authorizationService.EnsureAdminAsync();

        return await appointmentRepository.GetAllAsync(
            normalizedFilter.PatientId,
            normalizedFilter.DoctorId,
            ToStatuses(normalizedFilter.Status),
            normalizedFilter.From,
            normalizedFilter.To
        );
    }

    public async Task<IReadOnlyList<Appointment>> GetMineAsync(
        AppointmentFilterDto filter
    )
    {
        var normalizedFilter = await NormalizeFilterAsync(filter);
        var actor = await authorizationService.GetCurrentUserAsync();
        var statuses = ToStatuses(normalizedFilter.Status);

        if (actor.Role == UserRole.Patient)
        {
            return await appointmentRepository.GetByPatientIdAsync(
                actor.Id,
                statuses,
                normalizedFilter.From,
                normalizedFilter.To
            );
        }

        if (actor.Role == UserRole.Doctor)
        {
            var doctor = await authorizationService.GetCurrentDoctorAsync();

            return await appointmentRepository.GetByDoctorIdAsync(
                doctor.Id,
                statuses,
                normalizedFilter.From,
                normalizedFilter.To
            );
        }

        throw new ForbiddenException(
            AppointmentMessages.AppointmentAccessForbidden
        );
    }

    public async Task<IReadOnlyList<Appointment>> GetTodayAsync()
    {
        var dayStart = DateTime.UtcNow.Date;

        return await GetCurrentDoctorAppointmentsAsync(
            null,
            dayStart,
            dayStart.AddDays(1)
        );
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingAsync()
    {
        var doctor = await authorizationService.GetCurrentDoctorAsync();

        return await appointmentRepository.GetUpcomingByDoctorIdAsync(
            doctor.Id,
            AppointmentStatusPolicy.BlockingStatuses,
            DateTime.UtcNow
        );
    }

    public Task<IReadOnlyList<Appointment>> GetCompletedAsync()
    {
        return GetCurrentDoctorAppointmentsAsync(
            new[] { AppointmentStatus.Completed },
            null,
            null
        );
    }

    public Task<IReadOnlyList<Appointment>> GetCancelledAsync()
    {
        return GetCurrentDoctorAppointmentsAsync(
            new[] { AppointmentStatus.Cancelled },
            null,
            null
        );
    }

    private async Task<Appointment> GetAppointmentAsync(int id)
    {
        return await appointmentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(AppointmentMessages.NotFound(id));
    }

    private async Task<IReadOnlyList<Appointment>>
        GetCurrentDoctorAppointmentsAsync(
            IReadOnlyCollection<AppointmentStatus>? statuses,
            DateTime? from,
            DateTime? to
        )
    {
        var doctor = await authorizationService.GetCurrentDoctorAsync();

        return await appointmentRepository.GetByDoctorIdAsync(
            doctor.Id,
            statuses,
            from,
            to
        );
    }

    private async Task<AppointmentFilterDto> NormalizeFilterAsync(
        AppointmentFilterDto filter
    )
    {
        var normalizedFilter = filter with
        {
            From = AppointmentTimeHelper.NormalizeUtc(filter.From),
            To = AppointmentTimeHelper.NormalizeUtc(filter.To)
        };

        await filterValidator.EnsureValidAsync(normalizedFilter);

        return normalizedFilter;
    }

    private static IReadOnlyCollection<AppointmentStatus>? ToStatuses(
        AppointmentStatus? status
    )
    {
        return status.HasValue ? new[] { status.Value } : null;
    }
}
