using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.IServices;

namespace Vehicle.Api.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class VehicleController : ControllerBase
        {
            private readonly IVehicleService _vehicleService;

            public VehicleController(IVehicleService vehicleService)
            {
                _vehicleService = vehicleService;
            }

            // POST: api/vehicle
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] VehicleCreateDto dto)
            {
                var result = await _vehicleService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }

            // GET: api/vehicle/{id}
            [HttpGet("{id:guid}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var result = await _vehicleService.GetByIdAsync(id);
                if (result == null) return NotFound(new { Message = "Vehicle not found" });
                return Ok(result);
            }

            // GET: api/vehicle/customer/{customerId:guid}
            [HttpGet("customer/{customerId:guid}")]
            public async Task<IActionResult> GetByCustomerId(Guid customerId)
            {
                var result = await _vehicleService.GetByCustomerIdAsync(customerId);
                return Ok(result);
            }

            // GET: api/vehicle
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var result = await _vehicleService.GetAllAsync();
                return Ok(result);
            }

            // PUT: api/vehicle/{id}
            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] VehicleUpdateDto dto)
            {
                var result = await _vehicleService.UpdateAsync(id, dto);
                if (result == null) return NotFound(new { Message = "Vehicle not found" });
                return Ok(result);
            }

            // DELETE: api/vehicle/{id}
            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var success = await _vehicleService.DeleteAsync(id);
                if (!success) return NotFound(new { Message = "Vehicle not found" });
                return NoContent();
            }
        }
}
