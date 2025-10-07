using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace Staff.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);
        if (staff == null) return NotFound();
        return Ok(staff);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var staffs = await _staffService.GetAllStaffAsync();
        return Ok(staffs);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStaffDto dto)
    {
        var staff = await _staffService.CreateStaffAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = staff.StaffId }, staff);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffDto dto)
    {
        var updated = await _staffService.UpdateStaffAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _staffService.DeleteStaffAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
