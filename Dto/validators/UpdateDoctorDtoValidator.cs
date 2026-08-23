using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.DoctorValidators;

public class UpdateDoctorDtoValidator
    : AbstractValidator<UpdateDoctorDto>
{
    public UpdateDoctorDtoValidator()
    {
        this.AddFullNameRules(doctor => doctor.FullName);
        this.AddEmailRules(doctor => doctor.Email);

        RuleFor(doctor => doctor.DepartmentId)
            .GreaterThan(0)
            .WithMessage("A valid department is required.");
    }
}
