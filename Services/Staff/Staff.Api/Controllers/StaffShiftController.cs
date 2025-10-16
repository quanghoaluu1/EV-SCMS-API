using Application.DTOs;
using Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace Staff.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffShiftController : ControllerBase
{
    private readonly IStaffShiftService _service;

    public StaffShiftController(IStaffShiftService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess || result.Data == null) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("by-staff/{staffId}")]
    public async Task<IActionResult> GetByStaff(Guid staffId)
    {
        var result = await _service.GetByStaffAsync(staffId);
        return Ok(result);
    }

    [HttpGet("by-range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var result = await _service.GetByDateRangeAsync(start, end);
        return Ok(result);
    }

    [HttpGet("by-staff-range")]
    public async Task<IActionResult> GetByStaffRange([FromQuery] Guid staffId, [FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var result = await _service.GetByStaffAndDateRangeAsync(staffId, start, end);
        return Ok(result);
    }

    [HttpGet("available-staff")]
    public async Task<IActionResult> GetAvailableStaff([FromQuery] DateOnly start, [FromQuery] DateOnly end)
    {
        var result = await _service.GetAvailableStaffAsync(start, end);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStaffShiftDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            if (!result.IsSuccess || result.Data == null) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffShiftDto dto)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }
}


