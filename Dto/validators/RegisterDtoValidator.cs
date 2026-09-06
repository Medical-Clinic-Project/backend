using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using FluentValidation;

namespace backend.clinicalbackend.Dto.validators.AuthValidators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        this.AddFullNameRules(user => user.FullName);
        this.AddEmailRules(user => user.Email);
        this.AddPasswordRules(user => user.Password);
    }
}
