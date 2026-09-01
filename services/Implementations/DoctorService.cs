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
            $"Doctor with ID {id} was not found."
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
                "An account with this email already exists."
            );
        }

        var department =
            await departmentRepository.GetByIdForUpdateAsync(
                normalizedDto.DepartmentId
            )
            ?? throw new NotFoundException(
                $"Department with ID {normalizedDto.DepartmentId} " +
                "was not found."
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
                $"Doctor with ID {id} was not found."
            );

        var department =
            await departmentRepository.GetByIdForUpdateAsync(
                normalizedDto.DepartmentId
            )
            ?? throw new NotFoundException(
                $"Department with ID {normalizedDto.DepartmentId} " +
                "was not found."
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
                "An account with this email already exists."
            );
        }

        doctor.User.FullName = normalizedDto.FullName;
        doctor.User.Email = normalizedDto.Email;
        doctor.DepartmentId = department.Id;
        doctor.Department = department;

        await SaveChangesWithEmailConflictAsync(
            normalizedDto.Email,
            doctor.UserId
        );

        return doctor;
    }

    public async Task<Doctor> UpdateStatusAsync(
        int id,
        UpdateDoctorStatusDto dto
    )
    {
        var doctor =
            await doctorRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                $"Doctor with ID {id} was not found."
            );

        if (
            doctor.IsActive == dto.IsActive &&
            doctor.User.IsActive == dto.IsActive
        )
        {
            return doctor;
        }

        doctor.IsActive = dto.IsActive;
        doctor.User.IsActive = dto.IsActive;

        await doctorRepository.SaveChangesAsync();

        return doctor;
    }

    private static void EnsureDepartmentIsActive(
        Department department
    )
    {
        if (!department.IsActive)
        {
            throw new ConflictException(
                "Doctors can only be assigned to an active department."
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
                    "An account with this email already exists."
                );
            }
            throw;
        }
    }
}
