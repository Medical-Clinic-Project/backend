using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.DoctorAvailabilityValidators;

public class UpdateDoctorAvailabilityDtoValidator
    : AbstractValidator<UpdateDoctorAvailabilityDto>
{
    public UpdateDoctorAvailabilityDtoValidator()
    {
        RuleFor(availability => availability.StartTime)
            .NotEqual(default(DateTime))
            .WithMessage("Start time is required.");

        RuleFor(availability => availability.EndTime)
            .NotEqual(default(DateTime))
            .WithMessage("End time is required.")
            .GreaterThan(availability => availability.StartTime)
            .WithMessage("End time must be after start time.");
    }
}
