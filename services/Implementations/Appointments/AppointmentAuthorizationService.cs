using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Infrastructure.Interfaces;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Policies.Appointments;
using backend.clinicalbackend.repositories.Interfaces;

namespace backend.clinicalbackend.Services.Implementations.Appointments;

public sealed class AppointmentAuthorizationService(
    IUserRepository userRepository,
    IDoctorRepository doctorRepository,
    ICurrentUser currentUser
)
{
    public async Task<User> GetCurrentUserAsync()
    {
        return await userRepository.GetByIdAsync(currentUser.UserId)
            ?? throw new NotFoundException(
                AppointmentMessages.CurrentUserNotFound
            );
    }

    public async Task<User> GetCurrentPatientForBookingAsync()
    {
        var patient = await GetCurrentUserAsync();

        if (patient.Role != UserRole.Patient)
        {
            throw new ForbiddenException(
                AppointmentMessages.OnlyPatientsCanBook
            );
        }

        if (!patient.IsActive)
        {
            throw new ForbiddenException(
                AppointmentMessages.InactivePatientCannotBook
            );
        }

        return patient;
    }

    public async Task EnsureAdminAsync()
    {
        var actor = await GetCurrentUserAsync();

        if (actor.Role != UserRole.Admin)
        {
            throw new ForbiddenException(
                AppointmentMessages.AppointmentManagementForbidden
            );
        }
    }

    public async Task<Doctor> GetCurrentDoctorAsync()
    {
        return await GetDoctorForActorAsync(await GetCurrentUserAsync());
    }

    public Task EnsureCanViewAsync(Appointment appointment)
    {
        return EnsureAuthorizedAsync(
            appointment,
            AppointmentAuthorizationPolicy.CanView,
            AppointmentMessages.AppointmentAccessForbidden
        );
    }

    public Task EnsureCanCancelAsync(Appointment appointment)
    {
        return EnsureAuthorizedAsync(
            appointment,
            AppointmentAuthorizationPolicy.CanCancel,
            AppointmentMessages.AppointmentManagementForbidden
        );
    }

    public Task EnsureCanRescheduleAsync(Appointment appointment)
    {
        return EnsureAuthorizedAsync(
            appointment,
            AppointmentAuthorizationPolicy.CanReschedule,
            AppointmentMessages.AppointmentManagementForbidden,
            requireActiveDoctor: true
        );
    }

    public Task EnsureCanUpdateStatusAsync(Appointment appointment)
    {
        return EnsureAuthorizedAsync(
            appointment,
            AppointmentAuthorizationPolicy.CanUpdateStatus,
            AppointmentMessages.OnlyAssignedDoctorCanUpdateStatus,
            requireActiveDoctor: true
        );
    }

    private async Task EnsureAuthorizedAsync(
        Appointment appointment,
        Func<UserRole, int, int?, Appointment, bool> authorization,
        string message,
        bool requireActiveDoctor = false
    )
    {
        var actor = await GetCurrentUserAsync();
        int? doctorId = null;

        if (actor.Role == UserRole.Doctor)
        {
            var doctor = await GetDoctorForActorAsync(actor);

            if (
                requireActiveDoctor &&
                (!doctor.IsActive || !doctor.User.IsActive)
            )
            {
                throw new ForbiddenException(
                    AppointmentMessages.InactiveDoctorCannotManage
                );
            }

            doctorId = doctor.Id;
        }

        if (!authorization(actor.Role, actor.Id, doctorId, appointment))
        {
            throw new ForbiddenException(message);
        }
    }

    private async Task<Doctor> GetDoctorForActorAsync(User actor)
    {
        if (actor.Role != UserRole.Doctor)
        {
            throw new ForbiddenException(
                AppointmentMessages.AppointmentAccessForbidden
            );
        }

        return await doctorRepository.GetByUserIdAsync(actor.Id)
            ?? throw new NotFoundException(
                AppointmentMessages.DoctorProfileNotFound
            );
    }
}
