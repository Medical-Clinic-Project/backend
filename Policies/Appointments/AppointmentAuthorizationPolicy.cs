using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Policies.Appointments;

public static class AppointmentAuthorizationPolicy
{
    public static bool CanView(
        UserRole actorRole,
        int actorUserId,
        int? actorDoctorId,
        Appointment appointment
    )
    {
        return actorRole == UserRole.Admin ||
            (actorRole == UserRole.Patient &&
                appointment.PatientId == actorUserId) ||
            (actorRole == UserRole.Doctor &&
                appointment.DoctorId == actorDoctorId);
    }

    public static bool CanCancel(
        UserRole actorRole,
        int actorUserId,
        int? actorDoctorId,
        Appointment appointment
    )
    {
        return actorRole == UserRole.Admin ||
            (actorRole == UserRole.Patient &&
                appointment.PatientId == actorUserId);
    }

    public static bool CanReschedule(
        UserRole actorRole,
        int actorUserId,
        int? actorDoctorId,
        Appointment appointment
    )
    {
        return actorRole == UserRole.Admin ||
            (actorRole == UserRole.Patient &&
                appointment.PatientId == actorUserId) ||
            (actorRole == UserRole.Doctor &&
                appointment.DoctorId == actorDoctorId);
    }

    public static bool CanUpdateStatus(
        UserRole actorRole,
        int actorUserId,
        int? actorDoctorId,
        Appointment appointment
    )
    {
        return actorRole == UserRole.Admin ||
            (actorRole == UserRole.Doctor &&
                appointment.DoctorId == actorDoctorId);
    }
}
