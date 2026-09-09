using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Infrastructure.Interfaces;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace backend.clinicalbackend.Services.Implementations;

public class DoctorAvailabilityService(
    IDoctorAvailabilityRepository availabilityRepository,
    IDoctorRepository doctorRepository,
    ICurrentUser currentUser,
    IValidator<CreateDoctorAvailabilityDto> createValidator,
    IValidator<UpdateDoctorAvailabilityDto> updateValidator
) : IDoctorAvailabilityService
{
    public async Task<IReadOnlyList<DoctorAvailability>> GetMineAsync(
        DateTime? from,
        DateTime? to
    )
    {
        var range = NormalizeAndValidateRange(from, to);
        var doctor = await GetCurrentDoctorAsync();

        return await availabilityRepository.GetByDoctorIdAsync(
            doctor.Id,
            range.From,
            range.To
        );
    }

    public async Task<IReadOnlyList<DoctorAvailability>>
        GetByDoctorAsync(
            int doctorId,
            DateTime? from,
            DateTime? to
        )
    {
        var range = NormalizeAndValidateRange(from, to);
        var doctor = await doctorRepository.GetByIdAsync(doctorId)
            ?? throw new NotFoundException(
                $"Doctor with ID {doctorId} was not found."
            );

        if (!doctor.IsActive || !doctor.User.IsActive)
        {
            throw new ConflictException(
                "Availability is not available for an inactive doctor."
            );
        }

        return await availabilityRepository.GetByDoctorIdAsync(
            doctor.Id,
            range.From,
            range.To
        );
    }

    public async Task<DoctorAvailability> CreateAsync(
        CreateDoctorAvailabilityDto dto
    )
    {
        var normalizedDto = dto with
        {
            StartTime = NormalizeUtc(dto.StartTime),
            EndTime = NormalizeUtc(dto.EndTime)
        };

        await createValidator.EnsureValidAsync(normalizedDto);

        var doctor = await GetCurrentDoctorAsync();
        EnsureDoctorIsActive(doctor);
        EnsureStartTimeIsNotInPast(normalizedDto.StartTime);

        if (
            await availabilityRepository.HasOverlapAsync(
                doctor.Id,
                normalizedDto.StartTime,
                normalizedDto.EndTime
            )
        )
        {
            throw new ConflictException(
                "This availability overlaps an existing slot."
            );
        }

        var availability = new DoctorAvailability
        {
            DoctorId = doctor.Id,
            StartTime = normalizedDto.StartTime,
            EndTime = normalizedDto.EndTime
        };

        await availabilityRepository.AddAsync(availability);
        await availabilityRepository.SaveChangesAsync();

        return availability;
    }

    public async Task<DoctorAvailability> UpdateAsync(
        int id,
        UpdateDoctorAvailabilityDto dto
    )
    {
        var normalizedDto = dto with
        {
            StartTime = NormalizeUtc(dto.StartTime),
            EndTime = NormalizeUtc(dto.EndTime)
        };

        await updateValidator.EnsureValidAsync(normalizedDto);

        var doctor = await GetCurrentDoctorAsync();
        EnsureDoctorIsActive(doctor);

        var availability =
            await availabilityRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                $"Availability with ID {id} was not found."
            );

        EnsureOwnership(availability, doctor.Id);
        EnsureStartTimeIsNotInPast(normalizedDto.StartTime);

        if (
            await availabilityRepository.HasOverlapAsync(
                doctor.Id,
                normalizedDto.StartTime,
                normalizedDto.EndTime,
                availability.Id
            )
        )
        {
            throw new ConflictException(
                "This availability overlaps an existing slot."
            );
        }

        availability.StartTime = normalizedDto.StartTime;
        availability.EndTime = normalizedDto.EndTime;

        await availabilityRepository.SaveChangesAsync();

        return availability;
    }

    public async Task DeleteAsync(int id)
    {
        var doctor = await GetCurrentDoctorAsync();
        EnsureDoctorIsActive(doctor);

        var availability =
            await availabilityRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                $"Availability with ID {id} was not found."
            );

        EnsureOwnership(availability, doctor.Id);

        availabilityRepository.Delete(availability);
        await availabilityRepository.SaveChangesAsync();
    }

    private async Task<Doctor> GetCurrentDoctorAsync()
    {
        return await doctorRepository.GetByUserIdAsync(
            currentUser.UserId
        ) ?? throw new NotFoundException(
            "The doctor profile was not found."
        );
    }

    private static void EnsureDoctorIsActive(Doctor doctor)
    {
        if (!doctor.IsActive || !doctor.User.IsActive)
        {
            throw new ForbiddenException(
                "Inactive doctors cannot manage availability."
            );
        }
    }

    private static void EnsureStartTimeIsNotInPast(
        DateTime startTime
    )
    {
        if (startTime < DateTime.UtcNow)
        {
            throw new ConflictException(
                "Availability cannot start in the past."
            );
        }
    }

    private static void EnsureOwnership(
        DoctorAvailability availability,
        int doctorId
    )
    {
        if (availability.DoctorId != doctorId)
        {
            throw new ForbiddenException(
                "You can only modify your own availability."
            );
        }
    }

    private static (DateTime? From, DateTime? To)
        NormalizeAndValidateRange(
            DateTime? from,
            DateTime? to
        )
    {
        DateTime? normalizedFrom = from.HasValue
            ? NormalizeUtc(from.Value)
            : null;
        DateTime? normalizedTo = to.HasValue
            ? NormalizeUtc(to.Value)
            : null;

        var failures = new List<ValidationFailure>();

        if (normalizedFrom == default(DateTime))
        {
            failures.Add(new ValidationFailure(
                nameof(from),
                "From must be a valid date and time."
            ));
        }

        if (normalizedTo == default(DateTime))
        {
            failures.Add(new ValidationFailure(
                nameof(to),
                "To must be a valid date and time."
            ));
        }

        if (
            normalizedFrom.HasValue &&
            normalizedTo.HasValue &&
            normalizedTo.Value <= normalizedFrom.Value
        )
        {
            failures.Add(new ValidationFailure(
                nameof(to),
                "To must be after from."
            ));
        }

        if (failures.Count > 0)
        {
            throw new ValidationsException(failures);
        }

        return (normalizedFrom, normalizedTo);
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
