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
    IValidator<UpdatePatientProfileDto> profileValidator,
    IValidator<UpdatePatientStatusDto> statusValidator
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
            $"Patient with ID {id} was not found."
        );
    }

    public async Task<User> GetCurrentPatientAsync()
    {
        var patient = await patientRepository.GetByIdAsync(
            currentUser.UserId
        );

        return patient ?? throw new NotFoundException(
            "The patient profile was not found."
        );
    }

    public async Task<User> UpdateCurrentPatientAsync(
        UpdatePatientProfileDto dto
    )
    {
        var normalizedDto = dto with
        {
            FullName = dto.FullName?.Trim() ?? string.Empty,
            Email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty
        };

        await profileValidator.EnsureValidAsync(normalizedDto);

        var patient =
            await patientRepository.GetByIdForUpdateAsync(
                currentUser.UserId
            )
            ?? throw new NotFoundException(
                "The patient profile was not found."
            );

        if (
            await userRepository.EmailExistsAsync(
                normalizedDto.Email,
                patient.Id
            )
        )
        {
            throw new ConflictException(
                "An account with this email already exists."
            );
        }

        patient.FullName = normalizedDto.FullName;
        patient.Email = normalizedDto.Email;

        await SaveChangesWithEmailConflictAsync(
            normalizedDto.Email,
            patient.Id
        );

        return patient;
    }

    public async Task<User> UpdateStatusAsync(
        int id,
        UpdatePatientStatusDto dto
    )
    {
        await statusValidator.EnsureValidAsync(dto);

        var patient =
            await patientRepository.GetByIdForUpdateAsync(id)
            ?? throw new NotFoundException(
                $"Patient with ID {id} was not found."
            );

        var isActive = dto.IsActive!.Value;

        if (patient.IsActive == isActive)
        {
            return patient;
        }

        patient.IsActive = isActive;
        await patientRepository.SaveChangesAsync();

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
                    "An account with this email already exists."
                );
            }

            throw;
        }
    }
}
