using backend.clinicalbackend.Dto;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(
    IDashboardService dashboardService
) : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<AdminDashboardResponseDto>> GetAdmin()
    {
        var dashboard = await dashboardService.GetAdminDashboardAsync();

        return Ok(dashboard);
    }

    [HttpGet("doctor")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<DoctorDashboardResponseDto>> GetDoctor()
    {
        var dashboard = await dashboardService.GetDoctorDashboardAsync();

        return Ok(dashboard);
    }
}
