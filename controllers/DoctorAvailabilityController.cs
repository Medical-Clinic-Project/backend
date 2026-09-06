using backend.clinicalbackend.Dto;
using backend.clinicalbackend.Mappers;
using backend.clinicalbackend.models;
using backend.clinicalbackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.clinicalbackend.controllers;

[ApiController]
[Route("api/doctor-availability")]
public class DoctorAvailabilityController(
    IDoctorAvailabilityService availabilityService
) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<
        ActionResult<IReadOnlyList<DoctorAvailabilityResponseDto>>
    > GetMine(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null
    )
    {
        var availability = await availabilityService.GetMineAsync(
            from,
            to
        );

        return Ok(
            availability
                .Select(slot => slot.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("/api/doctors/{doctorId:int}/availability")]
    [Authorize(Roles = nameof(UserRole.Patient) + "," + nameof(UserRole.Admin))]
    public async Task<
        ActionResult<IReadOnlyList<DoctorAvailabilityResponseDto>>
    > GetByDoctor(
        int doctorId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null
    )
    {
        var availability =
            await availabilityService.GetByDoctorAsync(
                doctorId,
                from,
                to
            );

        return Ok(
            availability
                .Select(slot => slot.ToResponseDto())
                .ToList()
        );
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<DoctorAvailabilityResponseDto>>
        Create(CreateDoctorAvailabilityDto dto)
    {
        var availability = await availabilityService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetMine),
            availability.ToResponseDto()
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<DoctorAvailabilityResponseDto>>
        Update(
            int id,
            UpdateDoctorAvailabilityDto dto
        )
    {
        var availability = await availabilityService.UpdateAsync(
            id,
            dto
        );

        return Ok(availability.ToResponseDto());
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<IActionResult> Delete(int id)
    {
        await availabilityService.DeleteAsync(id);

        return NoContent();
    }
}
