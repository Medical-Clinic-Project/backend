using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;

namespace backend.clinicalbackend.Services.Implementations;

public class DepartmentService(
    IDepartmentRepository departmentRepository,
    IValidator<CreateDepartmentDto> createValidator,
    IValidator<UpdateDepartmentDto> updateValidator
) : IDepartmentService
{
    public async Task<IReadOnlyList<Department>> GetAllAsync(
        string? search
    )
    {
        return await departmentRepository.GetAllAsync(search);
    }

    public async Task<Department> GetByIdAsync(int id)
    {
        var department = await departmentRepository.GetByIdAsync(id);

        return department ?? throw new NotFoundException(
            $"Department with ID {id} was not found."
        );
    }

    public async Task<Department> CreateAsync(
        CreateDepartmentDto dto
    )
    {
        var normalizedDto = dto with
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            Description = dto.Description?.Trim() ?? string.Empty
        };

        await createValidator.EnsureValidAsync(normalizedDto);

        if (await departmentRepository.NameExistsAsync(normalizedDto.Name))
        {
            throw new ConflictException(
                "A department with this name already exists."
            );
        }

        var department = new Department
        {
            Name = normalizedDto.Name,
            Description = normalizedDto.Description,
            IsActive = normalizedDto.IsActive
        };

        await departmentRepository.AddAsync(department);
        await departmentRepository.SaveChangesAsync();

        return department;
    }

    public async Task<Department> UpdateAsync(
        int id,
        UpdateDepartmentDto dto
    )
    {
        var normalizedDto = dto with
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            Description = dto.Description?.Trim() ?? string.Empty
        };

        await updateValidator.EnsureValidAsync(normalizedDto);

        var department =
            await departmentRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                $"Department with ID {id} was not found."
            );

        if (
            await departmentRepository.NameExistsAsync(
                normalizedDto.Name,
                id
            )
        )
        {
            throw new ConflictException(
                "A department with this name already exists."
            );
        }

        department.Name = normalizedDto.Name;
        department.Description = normalizedDto.Description;
        department.IsActive = normalizedDto.IsActive;

        await departmentRepository.SaveChangesAsync();

        return department;
    }

    public async Task<Department> UpdateStatusAsync(
        int id,
        UpdateDepartmentStatusDto dto
    )
    {
        var department =
            await departmentRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                $"Department with ID {id} was not found."
            );

        if (department.IsActive == dto.IsActive)
        {
            return department;
        }

        department.IsActive = dto.IsActive;
        await departmentRepository.SaveChangesAsync();

        return department;
    }
}
