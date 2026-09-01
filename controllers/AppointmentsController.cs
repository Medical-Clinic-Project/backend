using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Mappers;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(
    IAppointmentService appointmentService
) : ControllerBase
{
    private const string PatientRole = nameof(UserRole.Patient);
    private const string DoctorRole = nameof(UserRole.Doctor);
    private const string AdminRole = nameof(UserRole.Admin);
    private const string PatientOrDoctorRoles =
        PatientRole + "," + DoctorRole;
    private const string PatientOrAdminRoles =
        PatientRole + "," + AdminRole;
    private const string DoctorOrAdminRoles =
        DoctorRole + "," + AdminRole;
    private const string PatientDoctorOrAdminRoles =
        PatientRole + "," + DoctorRole + "," + AdminRole;

    [HttpGet]
    [Authorize(Roles = AdminRole)]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>>
        GetAll(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] int? patientId = null,
            [FromQuery] AppointmentStatus? status = null
        )
    {
        var appointments = await appointmentService.GetAllAsync(
            new AppointmentFilterDto(
                from,
                to,
                doctorId,
                patientId,
                status
            )
        );

        return Ok(
            appointments
                .Select(appointment => appointment.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("mine")]
    [Authorize(Roles = PatientOrDoctorRoles)]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>>
        GetMine(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] AppointmentStatus? status = null
        )
    {
        var appointments = await appointmentService.GetMineAsync(
            new AppointmentFilterDto(from, to, null, null, status)
        );

        return Ok(
            appointments
                .Select(appointment => appointment.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("today")]
    [Authorize(Roles = DoctorRole)]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>>
        GetToday()
    {
        var appointments = await appointmentService.GetTodayAsync();

        return Ok(
            appointments
                .Select(appointment => appointment.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("upcoming")]
    [Authorize(Roles = DoctorRole)]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>>
        GetUpcoming()
    {
        var appointments = await appointmentService.GetUpcomingAsync();

        return Ok(
            appointments
                .Select(appointment => appointment.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("completed")]
    [Authorize(Roles = DoctorRole)]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>>
        GetCompleted()
    {
        var appointments = await appointmentService.GetCompletedAsync();

        return Ok(
            appointments
                .Select(appointment => appointment.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("cancelled")]
    [Authorize(Roles = DoctorRole)]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>>
        GetCancelled()
    {
        var appointments = await appointmentService.GetCancelledAsync();

        return Ok(
            appointments
                .Select(appointment => appointment.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = PatientDoctorOrAdminRoles)]
    public async Task<ActionResult<AppointmentResponseDto>> GetById(int id)
    {
        var appointment = await appointmentService.GetByIdAsync(id);

        return Ok(appointment.ToResponseDto());
    }

    [HttpPost]
    [Authorize(Roles = PatientRole)]
    public async Task<ActionResult<AppointmentResponseDto>> Create(
        CreateAppointmentDto dto
    )
    {
        var appointment = await appointmentService.CreateAsync(dto);
        var response = appointment.ToResponseDto();

        return CreatedAtAction(
            nameof(GetById),
            new { id = appointment.Id },
            response
        );
    }

    [HttpPatch("{id:int}/cancel")]
    [Authorize(Roles = PatientOrAdminRoles)]
    public async Task<ActionResult<AppointmentResponseDto>> Cancel(int id)
    {
        var appointment = await appointmentService.CancelAsync(id);

        return Ok(appointment.ToResponseDto());
    }

    [HttpPut("{id:int}/reschedule")]
    [Authorize(Roles = PatientDoctorOrAdminRoles)]
    public async Task<ActionResult<AppointmentResponseDto>> Reschedule(
        int id,
        RescheduleAppointmentDto dto
    )
    {
        var appointment = await appointmentService.RescheduleAsync(id, dto);

        return Ok(appointment.ToResponseDto());
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = DoctorOrAdminRoles)]
    public async Task<ActionResult<AppointmentResponseDto>> UpdateStatus(
        int id,
        UpdateAppointmentStatusDto dto
    )
    {
        var appointment = await appointmentService.UpdateStatusAsync(
            id,
            dto
        );

        return Ok(appointment.ToResponseDto());
    }
}
