using backend.clinicalbackend.constants.Appointments;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.AppointmentValidators;

public class AppointmentFilterDtoValidator
    : AbstractValidator<AppointmentFilterDto>
{
    public AppointmentFilterDtoValidator()
    {
        RuleFor(filter => filter.From)
            .Must(value => !value.HasValue || value.Value != default)
            .WithMessage(AppointmentMessages.FromInvalid);

        RuleFor(filter => filter.To)
            .Must(value => !value.HasValue || value.Value != default)
            .WithMessage(AppointmentMessages.ToInvalid)
            .Must((filter, to) =>
                !filter.From.HasValue ||
                !to.HasValue ||
                to.Value > filter.From.Value
            )
            .WithMessage(AppointmentMessages.ToMustBeAfterFrom);

        RuleFor(filter => filter.DoctorId)
            .Must(id => !id.HasValue || id.Value > 0)
            .WithMessage(AppointmentMessages.DoctorRequired);

        RuleFor(filter => filter.PatientId)
            .Must(id => !id.HasValue || id.Value > 0)
            .WithMessage(AppointmentMessages.PatientRequired);

        RuleFor(filter => filter.Status)
            .Must(status =>
                !status.HasValue ||
                Enum.IsDefined(typeof(AppointmentStatus), status.Value)
            )
            .WithMessage(AppointmentMessages.StatusInvalid);
    }
}
