using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Mappers;

public static class PatientMapper
{
    public static PatientResponseDto ToPatientResponseDto(
        this User patient
    )
    {
        return new PatientResponseDto(
            patient.Id,
            patient.FullName,
            patient.Email,
            patient.IsActive
        );
    }
}
