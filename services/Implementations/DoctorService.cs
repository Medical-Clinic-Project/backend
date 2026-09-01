using backend.clinicalbackend.constants.Auth;
using backend.clinicalbackend.constants.Departments;
using backend.clinicalbackend.constants.Doctors;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.exceptions;
using backend.clinicalbackend.Infrastructure.Interfaces;
using backend.clinicalbackend.models;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace backend.clinicalbackend.Services.Implementations;

public class DoctorService(
    IDoctorRepository doctorRepository,
    IUserRepository userRepository,
    IDepartmentRepository departmentRepository,
    IPasswordHasher passwordHasher,
    IValidator<CreateDoctorDto> createValidator,
    IValidator<UpdateDoctorDto> updateValidator
) : IDoctorService
{
    public async Task<IReadOnlyList<Doctor>> GetAllAsync(
        string? search,
        int? departmentId
    )
    {
        return await doctorRepository.GetAllAsync(
            search,
            departmentId
        );
    }

    public async Task<Doctor> GetByIdAsync(int id)
    {
        var doctor = await doctorRepository.GetByIdAsync(id);

        return doctor ?? throw new NotFoundException(
            DoctorMessages.NotFound(id)
        );
    }

    public async Task<IReadOnlyList<Doctor>> GetActiveForPatientsAsync(
        string? search,
        int? departmentId
    )
    {
        return await doctorRepository.GetActiveForPatientsAsync(
            search,
            departmentId
        );
    }

    public async Task<Doctor> GetActiveForPatientByIdAsync(int id)
    {
        var doctor = await doctorRepository.GetActiveForPatientByIdAsync(id);

        return doctor ?? throw new NotFoundException(
            $"Doctor with ID {id} was not found."
        );
    }

    public async Task<Doctor> CreateAsync(CreateDoctorDto dto)
    {
        var normalizedDto = dto with
        {
            FullName = dto.FullName?.Trim() ?? string.Empty,
            Email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty
        };

        await createValidator.EnsureValidAsync(normalizedDto);

        if (await userRepository.EmailExistsAsync(normalizedDto.Email))
        {
            throw new ConflictException(
                AuthMessages.EmailAlreadyExists
            );
        }

        var department =
            await departmentRepository.GetByIdForUpdateAsync(
                normalizedDto.DepartmentId
            )
            ?? throw new NotFoundException(
                DepartmentMessages.NotFound(normalizedDto.DepartmentId)
            );

        EnsureDepartmentIsActive(department);

        var user = new User
        {
            FullName = normalizedDto.FullName,
            Email = normalizedDto.Email,
            PasswordHash = passwordHasher.HashPassword(
                normalizedDto.Password
            ),
            Role = UserRole.Doctor,
            IsActive = true
        };

        var doctor = new Doctor
        {
            User = user,
            Department = department,
            DepartmentId = department.Id,
            IsActive = true
        };

        await doctorRepository.AddAsync(doctor);

        // One SaveChanges call atomically persists the new User/Doctor graph
        await SaveChangesWithEmailConflictAsync(normalizedDto.Email);

        return doctor;
    }

    public async Task<Doctor> UpdateAsync(
        int id,
        UpdateDoctorDto dto
    )
    {
        var normalizedDto = dto with
        {
            FullName = dto.FullName?.Trim() ?? string.Empty,
            Email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty
        };

        await updateValidator.EnsureValidAsync(normalizedDto);

        var doctor =
            await doctorRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                DoctorMessages.NotFound(id)
            );

        var department =
            await departmentRepository.GetByIdForUpdateAsync(
                normalizedDto.DepartmentId
            )
            ?? throw new NotFoundException(
                DepartmentMessages.NotFound(normalizedDto.DepartmentId)
            );

        EnsureDepartmentIsActive(department);

        if (
            await userRepository.EmailExistsAsync(
                normalizedDto.Email,
                doctor.UserId
            )
        )
        {
            throw new ConflictException(
                AuthMessages.EmailAlreadyExists
            );
        }

        doctor.User.FullName = normalizedDto.FullName;
        doctor.User.Email = normalizedDto.Email;
        doctor.DepartmentId = department.Id;
        doctor.Department = department;

        if (normalizedDto.IsActive.HasValue)
        {
            doctor.IsActive = normalizedDto.IsActive.Value;
            doctor.User.IsActive = normalizedDto.IsActive.Value;
        }

        await SaveChangesWithEmailConflictAsync(
            normalizedDto.Email,
            doctor.UserId
        );

        return doctor;
    }

    private static void EnsureDepartmentIsActive(
        Department department
    )
    {
        if (!department.IsActive)
        {
            throw new ConflictException(
                DoctorMessages.DepartmentMustBeActive
            );
        }
    }

    private async Task SaveChangesWithEmailConflictAsync(
        string email,
        int? excludedUserId = null
    )
    {
        try
        {
            await doctorRepository.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (
                await userRepository.EmailExistsAsync(
                    email,
                    excludedUserId
                )
            )
            {
                throw new ConflictException(
                    AuthMessages.EmailAlreadyExists
                );
            }
            throw;
        }
    }
}
