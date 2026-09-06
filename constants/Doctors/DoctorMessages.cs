namespace backend.clinicalbackend.constants.Doctors;

public static class DoctorMessages
{
    public const string DepartmentMustBeActive =
        "Doctors can only be assigned to an active department.";

    public static string NotFound(int id) =>
        $"Doctor with ID {id} was not found.";
}
