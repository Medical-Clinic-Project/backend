using backend.clinicalbackend.constants.Patients;
using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.PatientValidators;

public class UpdatePatientDtoValidator
    : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientDtoValidator()
    {
        When(
            patient =>
                patient.FullName is not null ||
                patient.Email is not null,
            () =>
            {
                this.AddFullNameRules(patient => patient.FullName!);
                this.AddEmailRules(patient => patient.Email!);
            }
        );

        RuleFor(patient => patient.IsActive)
            .NotNull()
            .WithMessage(PatientMessages.StatusRequired)
            .When(patient =>
                patient.FullName is null &&
                patient.Email is null
            );
    }
}
