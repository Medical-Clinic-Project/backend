namespace backend.clinicalbackend.constants.Dashboard;

public static class DashboardMessages
{
    public const string TotalsRequired =
        "Dashboard totals are required.";
    public const string SummaryRequired =
        "Dashboard summary is required.";
    public const string CountsCannotBeNegative =
        "Dashboard counts cannot be negative.";
    public const string StatusCountsRequired =
        "Appointment status counts are required.";
    public const string StatusCountsMustIncludeEveryStatus =
        "Appointment status counts must include each status exactly once.";
    public const string DepartmentCountsRequired =
        "Department appointment counts are required.";
    public const string DepartmentIdInvalid =
        "Department ID must be greater than zero.";
    public const string DepartmentNameRequired =
        "Department name is required.";
    public const string TimeSeriesRequired =
        "Appointment time series data is required.";
    public const string TimeSeriesDateRequired =
        "Appointment time series dates are required.";
    public const string TimeSeriesDateMustBeUtc =
        "Appointment time series dates must use UTC.";
    public const string TimeSeriesMustBeChronological =
        "Appointment time series must be ordered chronologically.";
}
