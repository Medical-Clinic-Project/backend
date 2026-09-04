using backend.clinicalbackend.constants.Dashboard;
using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Dto.validators;
using backend.clinicalbackend.Mappers;
using backend.clinicalbackend.repositories.Interfaces;
using backend.clinicalbackend.Services.Implementations.Appointments;
using backend.clinicalbackend.Services.Interfaces;
using FluentValidation;

namespace backend.clinicalbackend.Services.Implementations;

public class DashboardService(
    IDashboardRepository dashboardRepository,
    AppointmentAuthorizationService authorizationService,
    IValidator<AdminDashboardResponseDto> adminDashboardValidator,
    IValidator<DoctorDashboardResponseDto> doctorDashboardValidator
) : IDashboardService
{
    public async Task<AdminDashboardResponseDto> GetAdminDashboardAsync()
    {
        await authorizationService.EnsureAdminAsync();

        var (rangeStart, rangeEnd) = GetRecentAppointmentRange(
            DateTime.UtcNow
        );

        var totals = await dashboardRepository.GetAdminTotalsAsync();
        var statusCounts =
            await dashboardRepository.GetAppointmentStatusCountsAsync();
        var departmentCounts = await dashboardRepository
            .GetAppointmentCountsByDepartmentAsync();
        var dailyCounts = await dashboardRepository
            .GetAppointmentCountsByDayAsync(rangeStart, rangeEnd);

        var response = new AdminDashboardResponseDto(
            totals.ToDto(),
            statusCounts.ToDtos(),
            departmentCounts
                .Select(count => count.ToDto())
                .ToList(),
            dailyCounts
                .Select(count => count.ToDto())
                .ToList()
        );

        await adminDashboardValidator.EnsureValidAsync(response);

        return response;
    }

    public async Task<DoctorDashboardResponseDto> GetDoctorDashboardAsync()
    {
        var doctor = await authorizationService.GetCurrentDoctorAsync();
        var utcNow = DateTime.UtcNow;
        var (rangeStart, rangeEnd) = GetRecentAppointmentRange(utcNow);

        var summary = await dashboardRepository
            .GetDoctorAppointmentSummaryAsync(doctor.Id, utcNow);
        var statusCounts = await dashboardRepository
            .GetAppointmentStatusCountsAsync(doctor.Id);
        var dailyCounts = await dashboardRepository
            .GetAppointmentCountsByDayAsync(
                rangeStart,
                rangeEnd,
                doctor.Id
            );

        var timeSeries = dailyCounts
            .Select(count => count.ToDto())
            .ToList();

        var response = new DoctorDashboardResponseDto(
            summary.ToDto(),
            statusCounts.ToDtos(),
            timeSeries,
            timeSeries
        );

        await doctorDashboardValidator.EnsureValidAsync(response);

        return response;
    }

    private static (DateTime Start, DateTime End)
        GetRecentAppointmentRange(DateTime utcNow)
    {
        var currentDayStart = utcNow.Date;

        return (
            currentDayStart.AddDays(
                -(DashboardConstants.RecentAppointmentDays - 1)
            ),
            currentDayStart.AddDays(1)
        );
    }
}
