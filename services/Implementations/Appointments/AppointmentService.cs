using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Helpers.Appointments;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Policies.Appointments;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace backend.clinicalbackend.Services.Implementations.Appointments;

public class AppointmentService(
    IAppointmentRepository appointmentRepository,
    AppointmentReadService appointmentReadService,
    AppointmentSchedulingVerifier schedulingVerifier,
    AppointmentAuthorizationService authorizationService,
    IValidator<CreateAppointmentDto> createValidator,
    IValidator<RescheduleAppointmentDto> rescheduleValidator,
    IValidator<UpdateAppointmentStatusDto> statusValidator
) : IAppointmentService
{
    public async Task<Appointment> CreateAsync(CreateAppointmentDto dto)
    {
        var normalizedDto = dto with
        {
            StartTime = AppointmentTimeHelper.NormalizeUtc(dto.StartTime),
            EndTime = AppointmentTimeHelper.NormalizeUtc(dto.EndTime),
            Reason = NormalizeOptionalText(dto.Reason),
            Notes = NormalizeOptionalText(dto.Notes)
        };

        await createValidator.EnsureValidAsync(normalizedDto);

        return await ExecuteWriteAsync(
            async () =>
            {
                var patient =
                    await authorizationService
                        .GetCurrentPatientForBookingAsync();

                await schedulingVerifier.EnsureAvailableAsync(
                    patient.Id,
                    normalizedDto.DoctorId,
                    normalizedDto.StartTime,
                    normalizedDto.EndTime
                );

                var appointment = new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = normalizedDto.DoctorId,
                    StartTime = normalizedDto.StartTime,
                    EndTime = normalizedDto.EndTime,
                    Status = AppointmentStatus.Pending,
                    Reason = normalizedDto.Reason,
                    Notes = normalizedDto.Notes
                };

                await appointmentRepository.AddAsync(appointment);
                await appointmentRepository.SaveChangesAsync();

                return await appointmentRepository.GetByIdAsync(
                    appointment.Id
                ) ?? throw new NotFoundException(
                    AppointmentMessages.NotFound(appointment.Id)
                );
            }
        );
    }

    public Task<Appointment> GetByIdAsync(int id)
    {
        return appointmentReadService.GetByIdAsync(id);
    }

    public Task<IReadOnlyList<Appointment>> GetAllAsync(
        AppointmentFilterDto filter
    )
    {
        return appointmentReadService.GetAllAsync(filter);
    }

    public Task<IReadOnlyList<Appointment>> GetMineAsync(
        AppointmentFilterDto filter
    )
    {
        return appointmentReadService.GetMineAsync(filter);
    }

    public Task<IReadOnlyList<Appointment>> GetTodayAsync()
    {
        return appointmentReadService.GetTodayAsync();
    }

    public Task<IReadOnlyList<Appointment>> GetUpcomingAsync()
    {
        return appointmentReadService.GetUpcomingAsync();
    }

    public Task<IReadOnlyList<Appointment>> GetCompletedAsync()
    {
        return appointmentReadService.GetCompletedAsync();
    }

    public Task<IReadOnlyList<Appointment>> GetCancelledAsync()
    {
        return appointmentReadService.GetCancelledAsync();
    }

    public async Task<Appointment> CancelAsync(int id)
    {
        return await ExecuteWriteAsync(
            async () =>
            {
                var appointment = await GetAppointmentForUpdateAsync(id);
                await authorizationService.EnsureCanCancelAsync(appointment);

                if (!AppointmentStatusPolicy.CanBeCancelled(
                        appointment.Status,
                        appointment.StartTime,
                        DateTime.UtcNow
                    ))
                {
                    throw new ConflictException(
                        AppointmentMessages.AppointmentCannotBeCancelled
                    );
                }

                appointment.Status = AppointmentStatus.Cancelled;
                await appointmentRepository.SaveChangesAsync();

                return appointment;
            }
        );
    }

    public async Task<Appointment> RescheduleAsync(
        int id,
        RescheduleAppointmentDto dto
    )
    {
        var normalizedDto = dto with
        {
            StartTime = AppointmentTimeHelper.NormalizeUtc(dto.StartTime),
            EndTime = AppointmentTimeHelper.NormalizeUtc(dto.EndTime)
        };

        await rescheduleValidator.EnsureValidAsync(normalizedDto);

        return await ExecuteWriteAsync(
            async () =>
            {
                var appointment = await GetAppointmentForUpdateAsync(id);
                await authorizationService.EnsureCanRescheduleAsync(
                    appointment
                );

                if (!AppointmentStatusPolicy.CanBeRescheduled(
                        appointment.Status,
                        appointment.StartTime,
                        DateTime.UtcNow
                    ))
                {
                    throw new ConflictException(
                        AppointmentMessages.AppointmentCannotBeRescheduled
                    );
                }

                await schedulingVerifier.EnsureAvailableAsync(
                    appointment.PatientId,
                    appointment.DoctorId,
                    normalizedDto.StartTime,
                    normalizedDto.EndTime,
                    appointment.Id
                );

                appointment.StartTime = normalizedDto.StartTime;
                appointment.EndTime = normalizedDto.EndTime;

                await appointmentRepository.SaveChangesAsync();

                return appointment;
            }
        );
    }

    public async Task<Appointment> UpdateStatusAsync(
        int id,
        UpdateAppointmentStatusDto dto
    )
    {
        await statusValidator.EnsureValidAsync(dto);

        return await ExecuteWriteAsync(
            async () =>
            {
                var appointment = await GetAppointmentForUpdateAsync(id);
                await authorizationService.EnsureCanUpdateStatusAsync(
                    appointment
                );

                if (!AppointmentStatusPolicy.CanTransition(
                        appointment.Status,
                        dto.Status
                    ))
                {
                    throw new ConflictException(
                        AppointmentMessages.InvalidStatusTransition
                    );
                }

                if (
                    dto.Status == AppointmentStatus.Completed &&
                    !AppointmentTimeHelper.HasEnded(
                        appointment.EndTime,
                        DateTime.UtcNow
                    )
                )
                {
                    throw new ConflictException(
                        AppointmentMessages.AppointmentNotYetEnded
                    );
                }

                appointment.Status = dto.Status;
                await appointmentRepository.SaveChangesAsync();

                return appointment;
            }
        );
    }

    private async Task<T> ExecuteWriteAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            return await appointmentRepository.ExecuteInTransactionAsync(
                operation
            );
        }
        catch (SqliteException exception) when (
            exception.SqliteErrorCode is 5 or 6
        )
        {
            throw new ConflictException(
                AppointmentMessages.ConcurrentModification
            );
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqliteException
            {
                SqliteErrorCode: 5 or 6
            }
        )
        {
            throw new ConflictException(
                AppointmentMessages.ConcurrentModification
            );
        }
    }

    private async Task<Appointment> GetAppointmentForUpdateAsync(int id)
    {
        return await appointmentRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                AppointmentMessages.NotFound(id)
            );
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
