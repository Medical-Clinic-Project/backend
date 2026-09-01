using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.AppointmentValidators;

public class RescheduleAppointmentDtoValidator
    : AbstractValidator<RescheduleAppointmentDto>
{
    public RescheduleAppointmentDtoValidator()
    {
        RuleFor(appointment => appointment.StartTime)
            .NotEqual(default(DateTime))
            .WithMessage(AppointmentMessages.StartTimeRequired);

        RuleFor(appointment => appointment.EndTime)
            .NotEqual(default(DateTime))
            .WithMessage(AppointmentMessages.EndTimeRequired)
            .GreaterThan(appointment => appointment.StartTime)
            .WithMessage(AppointmentMessages.EndTimeMustBeAfterStartTime);
    }
}
