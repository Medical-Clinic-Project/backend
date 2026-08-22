using backend.clinicalbackend.Dto;
using backend.clinicalbackend.constants;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.DepartmentValidators;

public class CreateDepartmentDtoValidator
    : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(department => department.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MinimumLength(DepartmentConstants.NameMinimumLength)
            .WithMessage("Department name must be at least 2 characters.")
            .MaximumLength(DepartmentConstants.NameMaximumLength)
            .WithMessage("Department name must be 100 characters or fewer.");

        RuleFor(department => department.Description)
            .MaximumLength(DepartmentConstants.DescriptionMaximumLength)
            .WithMessage("Description must be 500 characters or fewer.");
    }
}
