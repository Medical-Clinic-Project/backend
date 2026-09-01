namespace backend.clinicalbackend.Helpers.Appointments;

public static class AppointmentTimeHelper
{
    public static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    public static DateTime? NormalizeUtc(DateTime? value)
    {
        return value.HasValue ? NormalizeUtc(value.Value) : null;
    }

    public static bool HasValidRange(
        DateTime startTime,
        DateTime endTime
    )
    {
        var normalizedStartTime = NormalizeUtc(startTime);
        var normalizedEndTime = NormalizeUtc(endTime);

        return normalizedStartTime != default &&
            normalizedEndTime != default &&
            normalizedEndTime > normalizedStartTime;
    }

    public static bool IsInPast(DateTime value, DateTime utcNow)
    {
        return NormalizeUtc(value) < NormalizeUtc(utcNow);
    }

    public static bool HasStarted(DateTime startTime, DateTime utcNow)
    {
        return NormalizeUtc(startTime) <= NormalizeUtc(utcNow);
    }

    public static bool HasEnded(DateTime endTime, DateTime utcNow)
    {
        return NormalizeUtc(endTime) <= NormalizeUtc(utcNow);
    }

    public static bool IsContainedWithin(
        DateTime startTime,
        DateTime endTime,
        DateTime containerStartTime,
        DateTime containerEndTime
    )
    {
        return HasValidRange(startTime, endTime) &&
            HasValidRange(containerStartTime, containerEndTime) &&
            NormalizeUtc(startTime) >= NormalizeUtc(containerStartTime) &&
            NormalizeUtc(endTime) <= NormalizeUtc(containerEndTime);
    }

    public static bool Overlaps(
        DateTime startTime,
        DateTime endTime,
        DateTime otherStartTime,
        DateTime otherEndTime
    )
    {
        return HasValidRange(startTime, endTime) &&
            HasValidRange(otherStartTime, otherEndTime) &&
            NormalizeUtc(startTime) < NormalizeUtc(otherEndTime) &&
            NormalizeUtc(endTime) > NormalizeUtc(otherStartTime);
    }
}
