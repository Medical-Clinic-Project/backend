using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.DoctorValidators;

public class CreateDoctorDtoValidator
    : AbstractValidator<CreateDoctorDto>
{
    public CreateDoctorDtoValidator()
    {
        this.AddFullNameRules(doctor => doctor.FullName);
        this.AddEmailRules(doctor => doctor.Email);
        this.AddPasswordRules(doctor => doctor.Password);

        RuleFor(doctor => doctor.DepartmentId)
            .GreaterThan(0)
            .WithMessage("A valid department is required.");
    }
}
