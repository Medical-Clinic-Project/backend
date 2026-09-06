using backend.clinicalbackend.constants.Auth;
using backend.clinicalbackend.constants.Patients;
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

public class PatientService(
    IPatientRepository patientRepository,
    IUserRepository userRepository,
    ICurrentUser currentUser,
    IValidator<UpdatePatientDto> updateValidator
) : IPatientService
{
    public async Task<IReadOnlyList<User>> GetAllAsync(
        string? search,
        bool? isActive
    )
    {
        return await patientRepository.GetAllAsync(
            search,
            isActive
        );
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var patient = await patientRepository.GetByIdAsync(id);

        return patient ?? throw new NotFoundException(
            PatientMessages.NotFound(id)
        );
    }

    public async Task<User> GetCurrentPatientAsync()
    {
        var patient = await patientRepository.GetByIdAsync(
            currentUser.UserId
        );

        return patient ?? throw new NotFoundException(
            PatientMessages.ProfileNotFound
        );
    }

    public async Task<User> UpdateAsync(
        int id,
        UpdatePatientDto dto
    )
    {
        var normalizedDto = dto with
        {
            FullName = dto.FullName?.Trim(),
            Email = dto.Email?.Trim().ToLowerInvariant()
        };

        await updateValidator.EnsureValidAsync(normalizedDto);

        var requestingUser =
            await userRepository.GetByIdAsync(currentUser.UserId);

        if (
            requestingUser is null ||
            (
                requestingUser.Role != UserRole.Admin &&
                requestingUser.Role != UserRole.Patient
            )
        )
        {
            throw new ForbiddenException(
                PatientMessages.UpdateNotAllowed
            );
        }

        var patient =
            await patientRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                PatientMessages.NotFound(id)
            );

        if (requestingUser.Role == UserRole.Admin)
        {
            if (
                normalizedDto.FullName is not null ||
                normalizedDto.Email is not null
            )
            {
                throw new ForbiddenException(
                    PatientMessages.UpdateNotAllowed
                );
            }

            var isActive = normalizedDto.IsActive!.Value;

            if (patient.IsActive == isActive)
            {
                return patient;
            }

            patient.IsActive = isActive;
            await patientRepository.SaveChangesAsync();

            return patient;
        }

        if (
            patient.Id != requestingUser.Id ||
            normalizedDto.IsActive.HasValue
        )
        {
            throw new ForbiddenException(
                PatientMessages.UpdateNotAllowed
            );
        }

        if (
            await userRepository.EmailExistsAsync(
                normalizedDto.Email!,
                patient.Id
            )
        )
        {
            throw new ConflictException(
                AuthMessages.EmailAlreadyExists
            );
        }

        patient.FullName = normalizedDto.FullName!;
        patient.Email = normalizedDto.Email!;

        await SaveChangesWithEmailConflictAsync(
            normalizedDto.Email!,
            patient.Id
        );

        return patient;
    }

    private async Task SaveChangesWithEmailConflictAsync(
        string email,
        int excludedUserId
    )
    {
        try
        {
            await patientRepository.SaveChangesAsync();
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
