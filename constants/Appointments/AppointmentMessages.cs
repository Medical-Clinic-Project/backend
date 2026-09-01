namespace backend.clinicalbackend.constants.Appointments;

public static class AppointmentMessages
{
    public const string DoctorRequired = "A valid doctor is required.";
    public const string PatientRequired = "A valid patient is required.";
    public const string StartTimeRequired = "Start time is required.";
    public const string EndTimeRequired = "End time is required.";
    public const string EndTimeMustBeAfterStartTime =
        "End time must be after start time.";
    public const string FromInvalid = "From must be a valid date and time.";
    public const string ToInvalid = "To must be a valid date and time.";
    public const string ToMustBeAfterFrom = "To must be after from.";
    public const string StatusInvalid = "Appointment status is invalid.";

    public static readonly string ReasonTooLong =
        $"Reason must be {AppointmentConstants.ReasonMaximumLength} " +
        "characters or fewer.";
    public static readonly string NotesTooLong =
        $"Notes must be {AppointmentConstants.NotesMaximumLength} " +
        "characters or fewer.";

    public const string PatientProfileNotFound =
        "The patient profile was not found.";
    public const string DoctorProfileNotFound =
        "The doctor profile was not found.";
    public const string CurrentUserNotFound =
        "The authenticated user was not found.";
    public const string InactivePatientCannotBook =
        "Inactive patients cannot book appointments.";
    public const string InactiveDoctorCannotBeBooked =
        "Appointments cannot be booked with an inactive doctor.";
    public const string InactiveDoctorCannotManage =
        "Inactive doctors cannot manage appointments.";
    public const string InactiveDepartmentCannotBeBooked =
        "Appointments cannot be booked in an inactive department.";
    public const string StartTimeCannotBeInPast =
        "Appointment start time cannot be in the past.";
    public const string OutsideDoctorAvailability =
        "The requested time is outside the doctor's availability.";
    public const string DoctorTimeConflict =
        "The doctor already has an appointment during this time.";
    public const string PatientTimeConflict =
        "You already have an appointment during this time.";
    public const string DuplicateAppointment =
        "An identical appointment already exists.";
    public const string ConcurrentModification =
        "The appointment changed while it was being updated. Please try again.";
    public const string AppointmentCannotBeCancelled =
        "This appointment cannot be cancelled.";
    public const string AppointmentCannotBeRescheduled =
        "This appointment cannot be rescheduled.";
    public const string AppointmentNotYetEnded =
        "An appointment cannot be completed before it has ended.";
    public const string InvalidStatusTransition =
        "This appointment status transition is not allowed.";
    public const string AppointmentAccessForbidden =
        "You are not allowed to access this appointment.";
    public const string AppointmentManagementForbidden =
        "You are not allowed to manage this appointment.";
    public const string OnlyPatientsCanBook =
        "Only patients can book appointments.";
    public const string OnlyAssignedDoctorCanUpdateStatus =
        "Only the assigned doctor can update this appointment status.";

    public static string NotFound(int id) =>
        $"Appointment with ID {id} was not found.";

    public static string DoctorNotFound(int id) =>
        $"Doctor with ID {id} was not found.";

    public static string PatientNotFound(int id) =>
        $"Patient with ID {id} was not found.";
}
