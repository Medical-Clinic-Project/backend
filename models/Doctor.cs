namespace backend.clinicalbackend.models;

public class Doctor
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public ICollection<DoctorAvailability> Availabilities { get; set; }
        = new List<DoctorAvailability>();

    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}
