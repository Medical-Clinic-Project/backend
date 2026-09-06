using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Mappers;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(
    IPatientService patientService
) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IReadOnlyList<PatientResponseDto>>>
        GetAll(
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null
        )
    {
        var patients = await patientService.GetAllAsync(
            search,
            isActive
        );

        return Ok(
            patients
                .Select(patient => patient.ToPatientResponseDto())
                .ToList()
        );
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<PatientResponseDto>> GetById(
        int id
    )
    {
        var patient = await patientService.GetByIdAsync(id);

        return Ok(patient.ToPatientResponseDto());
    }

    [HttpGet("me")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<ActionResult<PatientResponseDto>> GetCurrentPatient()
    {
        var patient = await patientService.GetCurrentPatientAsync();

        return Ok(patient.ToPatientResponseDto());
    }

    [HttpPut("{id:int}")]
    [Authorize(
        Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Patient)
    )]
    public async Task<ActionResult<PatientResponseDto>> Update(
        int id,
        UpdatePatientDto dto
    )
    {
        var patient = await patientService.UpdateAsync(id, dto);

        return Ok(patient.ToPatientResponseDto());
    }
}
