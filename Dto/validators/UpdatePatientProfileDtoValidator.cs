using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.PatientValidators;

public class UpdatePatientProfileDtoValidator
    : AbstractValidator<UpdatePatientProfileDto>
{
    public UpdatePatientProfileDtoValidator()
    {
        this.AddFullNameRules(patient => patient.FullName);
        this.AddEmailRules(patient => patient.Email);
    }
}
