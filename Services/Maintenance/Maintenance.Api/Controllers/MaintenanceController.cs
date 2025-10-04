using Maintenance.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using static Maintenance.Application.DTOs.MaintenanceDTOs;

namespace Maintenance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        // GET: api/maintenance
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var maintenances = await _maintenanceService.GetAllAsync();
            return Ok(maintenances);
        }

        // GET: api/maintenance/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var maintenance = await _maintenanceService.GetByIdAsync(id);
            if (maintenance == null) return NotFound();
            return Ok(maintenance);
        }

        // POST: api/maintenance
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMaintenanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _maintenanceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.MaintenanceId }, created);
        }

        // PUT: api/maintenance/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaintenanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _maintenanceService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/maintenance/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _maintenanceService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
