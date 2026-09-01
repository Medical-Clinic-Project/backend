using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.AppointmentValidators;

public class CreateAppointmentDtoValidator
    : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentDtoValidator()
    {
        RuleFor(appointment => appointment.DoctorId)
            .GreaterThan(0)
            .WithMessage(AppointmentMessages.DoctorRequired);

        AddTimeRangeRules(
            appointment => appointment.StartTime,
            appointment => appointment.EndTime
        );

        RuleFor(appointment => appointment.Reason)
            .MaximumLength(AppointmentConstants.ReasonMaximumLength)
            .WithMessage(AppointmentMessages.ReasonTooLong);

        RuleFor(appointment => appointment.Notes)
            .MaximumLength(AppointmentConstants.NotesMaximumLength)
            .WithMessage(AppointmentMessages.NotesTooLong);
    }

    private void AddTimeRangeRules(
        System.Linq.Expressions.Expression<
            Func<CreateAppointmentDto, DateTime>
        > startTime,
        System.Linq.Expressions.Expression<
            Func<CreateAppointmentDto, DateTime>
        > endTime
    )
    {
        RuleFor(startTime)
            .NotEqual(default(DateTime))
            .WithMessage(AppointmentMessages.StartTimeRequired);

        RuleFor(endTime)
            .NotEqual(default(DateTime))
            .WithMessage(AppointmentMessages.EndTimeRequired)
            .GreaterThan(startTime)
            .WithMessage(AppointmentMessages.EndTimeMustBeAfterStartTime);
    }
}
