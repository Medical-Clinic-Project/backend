using backend.clinicalbackend.Dto;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.PatientValidators;

public class UpdatePatientStatusDtoValidator
    : AbstractValidator<UpdatePatientStatusDto>
{
    public UpdatePatientStatusDtoValidator()
    {
        RuleFor(patient => patient.IsActive)
            .NotNull()
            .WithMessage("Patient status is required.");
    }
}
