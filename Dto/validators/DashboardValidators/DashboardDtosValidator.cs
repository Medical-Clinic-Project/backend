using backend.clinicalbackend.constants.Dashboard;
using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.DashboardValidators;

public sealed class AdminDashboardTotalsDtoValidator
    : AbstractValidator<AdminDashboardTotalsDto>
{
    public AdminDashboardTotalsDtoValidator()
    {
        RuleFor(totals => totals.TotalPatients)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);

        RuleFor(totals => totals.TotalDoctors)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);

        RuleFor(totals => totals.TotalDepartments)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);

        RuleFor(totals => totals.TotalAppointments)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);
    }
}

public sealed class AppointmentStatusCountDtoValidator
    : AbstractValidator<AppointmentStatusCountDto>
{
    public AppointmentStatusCountDtoValidator()
    {
        RuleFor(count => count.Status)
            .IsInEnum()
            .WithMessage(AppointmentMessages.StatusInvalid);

        RuleFor(count => count.Count)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);
    }
}

public sealed class DepartmentAppointmentCountDtoValidator
    : AbstractValidator<DepartmentAppointmentCountDto>
{
    public DepartmentAppointmentCountDtoValidator()
    {
        RuleFor(count => count.DepartmentId)
            .GreaterThan(0)
            .WithMessage(DashboardMessages.DepartmentIdInvalid);

        RuleFor(count => count.DepartmentName)
            .NotEmpty()
            .WithMessage(DashboardMessages.DepartmentNameRequired);

        RuleFor(count => count.AppointmentCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);
    }
}

public sealed class AppointmentTimeSeriesPointDtoValidator
    : AbstractValidator<AppointmentTimeSeriesPointDto>
{
    public AppointmentTimeSeriesPointDtoValidator()
    {
        RuleFor(point => point.Date)
            .NotEqual(default(DateTime))
            .WithMessage(DashboardMessages.TimeSeriesDateRequired)
            .Must(date => date.Kind == DateTimeKind.Utc)
            .WithMessage(DashboardMessages.TimeSeriesDateMustBeUtc);

        RuleFor(point => point.AppointmentCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);
    }
}

public sealed class DoctorDashboardSummaryDtoValidator
    : AbstractValidator<DoctorDashboardSummaryDto>
{
    public DoctorDashboardSummaryDtoValidator()
    {
        RuleFor(summary => summary.TodayAppointments)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);

        RuleFor(summary => summary.UpcomingAppointments)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);

        RuleFor(summary => summary.CompletedAppointments)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);

        RuleFor(summary => summary.CancelledAppointments)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DashboardMessages.CountsCannotBeNegative);
    }
}

public sealed class AdminDashboardResponseDtoValidator
    : AbstractValidator<AdminDashboardResponseDto>
{
    public AdminDashboardResponseDtoValidator()
    {
        RuleFor(response => response.Totals)
            .NotNull()
            .WithMessage(DashboardMessages.TotalsRequired);
        RuleFor(response => response.Totals)
            .SetValidator(new AdminDashboardTotalsDtoValidator());

        this.AddStatusCountRules(
            response => response.AppointmentsByStatus
        );
        this.AddDepartmentCountRules(
            response => response.AppointmentsByDepartment
        );
        this.AddTimeSeriesRules(
            response => response.AppointmentsOverTime
        );
    }
}

public sealed class DoctorDashboardResponseDtoValidator
    : AbstractValidator<DoctorDashboardResponseDto>
{
    public DoctorDashboardResponseDtoValidator()
    {
        RuleFor(response => response.Summary)
            .NotNull()
            .WithMessage(DashboardMessages.SummaryRequired);
        RuleFor(response => response.Summary)
            .SetValidator(new DoctorDashboardSummaryDtoValidator());

        this.AddStatusCountRules(
            response => response.AppointmentsByStatus
        );
        this.AddTimeSeriesRules(
            response => response.AppointmentsByDay
        );
        this.AddTimeSeriesRules(
            response => response.AppointmentsOverTime
        );
    }
}

public static class DashboardResponseValidatorExtensions
{
    public static void AddStatusCountRules<T>(
        this AbstractValidator<T> validator,
        System.Linq.Expressions.Expression<
            Func<T, IEnumerable<AppointmentStatusCountDto>>
        > propertyExpression
    )
    {
        validator.RuleFor(propertyExpression)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(DashboardMessages.StatusCountsRequired)
            .Must(IncludeEachAppointmentStatusExactlyOnce)
            .WithMessage(
                DashboardMessages.StatusCountsMustIncludeEveryStatus
            );

        validator.RuleForEach(propertyExpression)
            .SetValidator(new AppointmentStatusCountDtoValidator());
    }

    public static void AddDepartmentCountRules<T>(
        this AbstractValidator<T> validator,
        System.Linq.Expressions.Expression<
            Func<T, IEnumerable<DepartmentAppointmentCountDto>>
        > propertyExpression
    )
    {
        validator.RuleFor(propertyExpression)
            .NotNull()
            .WithMessage(DashboardMessages.DepartmentCountsRequired);

        validator.RuleForEach(propertyExpression)
            .SetValidator(new DepartmentAppointmentCountDtoValidator());
    }

    public static void AddTimeSeriesRules<T>(
        this AbstractValidator<T> validator,
        System.Linq.Expressions.Expression<
            Func<T, IEnumerable<AppointmentTimeSeriesPointDto>>
        > propertyExpression
    )
    {
        validator.RuleFor(propertyExpression)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(DashboardMessages.TimeSeriesRequired)
            .Must(BeChronologicallyOrdered)
            .WithMessage(
                DashboardMessages.TimeSeriesMustBeChronological
            );

        validator.RuleForEach(propertyExpression)
            .SetValidator(new AppointmentTimeSeriesPointDtoValidator());
    }

    private static bool IncludeEachAppointmentStatusExactlyOnce(
        IEnumerable<AppointmentStatusCountDto>? statusCounts
    )
    {
        var expectedStatuses = Enum.GetValues<AppointmentStatus>();

        return statusCounts is not null &&
            statusCounts.Count() == expectedStatuses.Length &&
            expectedStatuses.All(status =>
                statusCounts.Count(count => count.Status == status) == 1
            );
    }

    private static bool BeChronologicallyOrdered(
        IEnumerable<AppointmentTimeSeriesPointDto>? timeSeries
    )
    {
        return timeSeries is not null &&
            timeSeries
                .Zip(
                    timeSeries.Skip(1),
                    (previous, current) => previous.Date < current.Date
                )
                .All(isOrdered => isOrdered);
    }
}
