namespace backend.clinicalbackend.models;

public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Doctor> Doctors { get; set; }
        = new List<Doctor>();
}
