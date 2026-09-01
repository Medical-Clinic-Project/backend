using backend.clinicalbackend.models;
using backend.clinicalbackend.Helpers.Appointments;

namespace backend.clinicalbackend.Policies.Appointments;

public static class AppointmentStatusPolicy
{
    public static IReadOnlyCollection<AppointmentStatus>
        BlockingStatuses { get; } = Array.AsReadOnly(
            new[]
            {
                AppointmentStatus.Pending,
                AppointmentStatus.Confirmed
            }
        );

    public static bool CanTransition(
        AppointmentStatus currentStatus,
        AppointmentStatus nextStatus
    )
    {
        return currentStatus switch
        {
            AppointmentStatus.Pending => nextStatus is
                AppointmentStatus.Confirmed or
                AppointmentStatus.Cancelled,
            AppointmentStatus.Confirmed => nextStatus is
                AppointmentStatus.Completed or
                AppointmentStatus.Cancelled,
            _ => false
        };
    }

    public static bool CanBeCancelled(
        AppointmentStatus status,
        DateTime startTime,
        DateTime utcNow
    )
    {
        return (status is AppointmentStatus.Pending or
            AppointmentStatus.Confirmed) &&
            !AppointmentTimeHelper.HasStarted(startTime, utcNow);
    }

    public static bool CanBeRescheduled(
        AppointmentStatus status,
        DateTime startTime,
        DateTime utcNow
    )
    {
        return (status is AppointmentStatus.Pending or
            AppointmentStatus.Confirmed) &&
            !AppointmentTimeHelper.HasStarted(startTime, utcNow);
    }

    public static bool BlocksSchedulingSlot(AppointmentStatus status)
    {
        return BlockingStatuses.Contains(status);
    }
}
