using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Mappers;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class DoctorsController(
    IDoctorService doctorService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DoctorResponseDto>>>
        GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int? departmentId = null
        )
    {
        var doctors = await doctorService.GetAllAsync(
            search,
            departmentId
        );

        return Ok(
            doctors
                .Select(doctor => doctor.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorResponseDto>> GetById(
        int id
    )
    {
        var doctor = await doctorService.GetByIdAsync(id);

        return Ok(doctor.ToResponseDto());
    }

    [HttpPost]
    public async Task<ActionResult<DoctorResponseDto>> Create(
        CreateDoctorDto dto
    )
    {
        var doctor = await doctorService.CreateAsync(dto);
        var response = doctor.ToResponseDto();

        return CreatedAtAction(
            nameof(GetById),
            new { id = doctor.Id },
            response
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DoctorResponseDto>> Update(
        int id,
        UpdateDoctorDto dto
    )
    {
        var doctor = await doctorService.UpdateAsync(id, dto);

        return Ok(doctor.ToResponseDto());
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<DoctorResponseDto>> UpdateStatus(
        int id,
        UpdateDoctorStatusDto dto
    )
    {
        var doctor = await doctorService.UpdateStatusAsync(id, dto);

        return Ok(doctor.ToResponseDto());
    }
}
