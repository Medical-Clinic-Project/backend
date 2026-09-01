namespace backend.clinicalbackend.models;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public User Patient { get; set; } = null!;

    public int DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; } =
        AppointmentStatus.Pending;

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}
