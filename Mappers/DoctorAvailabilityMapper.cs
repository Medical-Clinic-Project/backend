using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Mappers;

public static class DoctorAvailabilityMapper
{
    public static DoctorAvailabilityResponseDto ToResponseDto(
        this DoctorAvailability availability
    )
    {
        return new DoctorAvailabilityResponseDto(
            availability.Id,
            availability.DoctorId,
            AsUtc(availability.StartTime),
            AsUtc(availability.EndTime)
        );
    }

    private static DateTime AsUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
