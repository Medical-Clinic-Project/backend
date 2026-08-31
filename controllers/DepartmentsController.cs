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
public class DepartmentsController(
    IDepartmentService departmentService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentResponseDto>>>
        GetAll([FromQuery] string? search = null)
    {
        var departments = await departmentService.GetAllAsync(search);

        return Ok(
            departments
                .Select(department => department.ToResponseDto())
                .ToList()
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentResponseDto>> GetById(
        int id
    )
    {
        var department = await departmentService.GetByIdAsync(id);

        return Ok(department.ToResponseDto());
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponseDto>> Create(
        CreateDepartmentDto dto
    )
    {
        var department = await departmentService.CreateAsync(dto);
        var response = department.ToResponseDto();

        return CreatedAtAction(
            nameof(GetById),
            new { id = department.Id },
            response
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentResponseDto>> Update(
        int id,
        UpdateDepartmentDto dto
    )
    {
        var department = await departmentService.UpdateAsync(id, dto);

        return Ok(department.ToResponseDto());
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<DepartmentResponseDto>> UpdateStatus(
        int id,
        UpdateDepartmentStatusDto dto
    )
    {
        var department =
            await departmentService.UpdateStatusAsync(id, dto);

        return Ok(department.ToResponseDto());
    }
}
