using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Helpers.Appointments;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Policies.Appointments;
using backend.clinicalbackend.repositories.Interfaces;

namespace backend.clinicalbackend.Services.Implementations.Appointments;

public sealed class AppointmentSchedulingVerifier(
    IAppointmentRepository appointmentRepository,
    IUserRepository userRepository,
    IDoctorRepository doctorRepository,
    IDoctorAvailabilityRepository availabilityRepository
)
{
    public async Task EnsureAvailableAsync(
        int patientId,
        int doctorId,
        DateTime startTime,
        DateTime endTime,
        int? excludedAppointmentId = null
    )
    {
        if (!AppointmentSchedulingPolicy.HasValidAppointmentWindow(
                startTime,
                endTime,
                DateTime.UtcNow
            ))
        {
            throw new ConflictException(
                AppointmentMessages.StartTimeCannotBeInPast
            );
        }

        var patient = await userRepository.GetByIdAsync(patientId);

        if (patient is null || patient.Role != UserRole.Patient)
        {
            throw new NotFoundException(
                AppointmentMessages.PatientNotFound(patientId)
            );
        }

        if (!patient.IsActive)
        {
            throw new ConflictException(
                AppointmentMessages.InactivePatientCannotBook
            );
        }

        var doctor = await doctorRepository.GetByIdAsync(doctorId)
            ?? throw new NotFoundException(
                AppointmentMessages.DoctorNotFound(doctorId)
            );

        if (!doctor.IsActive || !doctor.User.IsActive)
        {
            throw new ConflictException(
                AppointmentMessages.InactiveDoctorCannotBeBooked
            );
        }

        if (!doctor.Department.IsActive)
        {
            throw new ConflictException(
                AppointmentMessages.InactiveDepartmentCannotBeBooked
            );
        }

        var availabilities =
            await availabilityRepository.GetByDoctorIdAsync(
                doctorId,
                startTime,
                endTime
            );

        if (!availabilities.Any(availability =>
                AppointmentSchedulingPolicy.FitsWithinAvailability(
                    startTime,
                    endTime,
                    availability.StartTime,
                    availability.EndTime
                )
            ))
        {
            throw new ConflictException(
                AppointmentMessages.OutsideDoctorAvailability
            );
        }

        var blockingStatuses = AppointmentStatusPolicy.BlockingStatuses;

        if (await appointmentRepository.HasDuplicateAppointmentAsync(
                patientId,
                doctorId,
                startTime,
                endTime,
                blockingStatuses,
                excludedAppointmentId
            ))
        {
            throw new ConflictException(
                AppointmentMessages.DuplicateAppointment
            );
        }

        if (await appointmentRepository.HasDoctorOverlapAsync(
                doctorId,
                startTime,
                endTime,
                blockingStatuses,
                excludedAppointmentId
            ))
        {
            throw new ConflictException(
                AppointmentMessages.DoctorTimeConflict
            );
        }

        if (await appointmentRepository.HasPatientOverlapAsync(
                patientId,
                startTime,
                endTime,
                blockingStatuses,
                excludedAppointmentId
            ))
        {
            throw new ConflictException(
                AppointmentMessages.PatientTimeConflict
            );
        }
    }
}
