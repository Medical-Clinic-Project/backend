using backend.clinicalbackend.Dto;
using backend.clinicalbackend.constants;
using backend.clinicalbackend.constants.Departments;

using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.DepartmentValidators;

public class UpdateDepartmentDtoValidator
    : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(department => department.Name)
            .NotEmpty()
            .WithMessage(DepartmentMessages.NameRequired)
            .MinimumLength(DepartmentConstants.NameMinimumLength)
            .WithMessage(DepartmentMessages.NameTooShort)
            .MaximumLength(DepartmentConstants.NameMaximumLength)
            .WithMessage(DepartmentMessages.NameTooLong);

        RuleFor(department => department.Description)
            .MaximumLength(DepartmentConstants.DescriptionMaximumLength)
            .WithMessage(DepartmentMessages.DescriptionTooLong);

    }
}
