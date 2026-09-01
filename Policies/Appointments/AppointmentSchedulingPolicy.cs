using backend.clinicalbackend.Helpers.Appointments;

namespace backend.clinicalbackend.Policies.Appointments;

public static class AppointmentSchedulingPolicy
{
    public static bool HasValidAppointmentWindow(
        DateTime startTime,
        DateTime endTime,
        DateTime utcNow
    )
    {
        return AppointmentTimeHelper.HasValidRange(startTime, endTime) &&
            !AppointmentTimeHelper.IsInPast(startTime, utcNow);
    }

    public static bool FitsWithinAvailability(
        DateTime appointmentStartTime,
        DateTime appointmentEndTime,
        DateTime availabilityStartTime,
        DateTime availabilityEndTime
    )
    {
        return AppointmentTimeHelper.IsContainedWithin(
            appointmentStartTime,
            appointmentEndTime,
            availabilityStartTime,
            availabilityEndTime
        );
    }

    public static bool HasTimeOverlap(
        DateTime firstStartTime,
        DateTime firstEndTime,
        DateTime secondStartTime,
        DateTime secondEndTime
    )
    {
        return AppointmentTimeHelper.Overlaps(
            firstStartTime,
            firstEndTime,
            secondStartTime,
            secondEndTime
        );
    }
}
