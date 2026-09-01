using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Mappers;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/patient/doctors")]
[Authorize(Roles = nameof(UserRole.Patient))]
public class PatientDoctorsController(
    IDoctorService doctorService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientDoctorResponseDto>>>
        GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int? departmentId = null
        )
    {
        var doctors = await doctorService.GetActiveForPatientsAsync(
            search,
            departmentId
        );

        return Ok(
            doctors
                .Select(doctor => doctor.ToPatientResponseDto())
                .ToList()
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientDoctorResponseDto>> GetById(int id)
    {
        var doctor = await doctorService.GetActiveForPatientByIdAsync(id);

        return Ok(doctor.ToPatientResponseDto());
    }
}
