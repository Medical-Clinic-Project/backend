using backend.clinicalbackend.Dto;

namespace backend.clinicalbackend.Services.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardResponseDto> GetAdminDashboardAsync();

    Task<DoctorDashboardResponseDto> GetDoctorDashboardAsync();
}
