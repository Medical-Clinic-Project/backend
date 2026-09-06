namespace backend.clinicalbackend.constants.Patients;

public static class PatientMessages
{
    public const string ProfileNotFound =
        "The patient profile was not found.";

    public const string StatusRequired =
        "Patient status is required.";

    public const string UpdateNotAllowed =
        "You are not authorized to update this patient.";

    public static string NotFound(int id) =>
        $"Patient with ID {id} was not found.";
}
