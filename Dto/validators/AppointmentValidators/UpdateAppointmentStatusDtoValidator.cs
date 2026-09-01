using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.AppointmentValidators;

public class UpdateAppointmentStatusDtoValidator
    : AbstractValidator<UpdateAppointmentStatusDto>
{
    public UpdateAppointmentStatusDtoValidator()
    {
        RuleFor(appointment => appointment.Status)
            .IsInEnum()
            .WithMessage(AppointmentMessages.StatusInvalid);
    }
}
