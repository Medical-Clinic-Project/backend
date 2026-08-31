using backend.clinicalbackend.constants;

namespace backend.clinicalbackend.constants.Departments;

public static class DepartmentMessages
{
    public const string NameRequired = "Department name is required.";
    public static readonly string NameTooShort =
        $"Department name must be at least {DepartmentConstants.NameMinimumLength} characters.";
    public static readonly string NameTooLong =
        $"Department name must be {DepartmentConstants.NameMaximumLength} characters or fewer.";
    public static readonly string DescriptionTooLong =
        $"Description must be {DepartmentConstants.DescriptionMaximumLength} characters or fewer.";
    public const string NameAlreadyExists =
        "A department with this name already exists.";

    public static string NotFound(int id) =>
        $"Department with ID {id} was not found.";
}
