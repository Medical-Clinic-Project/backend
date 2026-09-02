using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Helpers.Appointments;
using backend.clinicalbackend.models;

namespace backend.clinicalbackend.Mappers;

public static class AppointmentMapper
{
    public static AppointmentResponseDto ToResponseDto(
        this Appointment appointment
    )
    {
        return new AppointmentResponseDto(
            appointment.Id,
            appointment.PatientId,
            appointment.Patient.FullName,
            appointment.Patient.Email,
            appointment.DoctorId,
            appointment.Doctor.User.FullName,
            appointment.Doctor.DepartmentId,
            appointment.Doctor.Department.Name,
            AppointmentTimeHelper.NormalizeUtc(appointment.StartTime),
            AppointmentTimeHelper.NormalizeUtc(appointment.EndTime),
            appointment.Status,
            appointment.Reason,
            appointment.Notes
        );
    }
}
