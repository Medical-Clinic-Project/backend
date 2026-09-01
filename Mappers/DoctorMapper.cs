using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Mappers;

public static class DoctorMapper
{
    public static DoctorResponseDto ToResponseDto(
        this Doctor doctor
    )
    {
        return new DoctorResponseDto(
            doctor.Id,
            doctor.User.FullName,
            doctor.User.Email,
            doctor.DepartmentId,
            doctor.Department.ToResponseDto(),
            doctor.IsActive
        );
    }

    public static PatientDoctorResponseDto ToPatientResponseDto(
        this Doctor doctor
    )
    {
        return new PatientDoctorResponseDto(
            doctor.Id,
            doctor.User.FullName,
            doctor.DepartmentId,
            doctor.Department.Name
        );
    }
}
