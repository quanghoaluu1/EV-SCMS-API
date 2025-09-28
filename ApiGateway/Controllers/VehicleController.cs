using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleGrpcClient _vehicleClient;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleGrpcClient vehicleClient, ILogger<VehicleController> logger)
        {
            _vehicleClient = vehicleClient;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(Guid id)
        {
            try
            {
                var response = await _vehicleClient.GetVehicleAsync(id);

                if (!response.Success)
                    return NotFound(response.Message);

                return Ok(response.Vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var response = await _vehicleClient.GetAllVehiclesAsync();

                if (!response.Success)
                    return BadRequest(response.Message);

                return Ok(response.Vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all vehicles");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            try
            {
                var response = await _vehicleClient.GetByCustomerIdAsync(customerId);

                if (!response.Success)
                    return BadRequest(response.Message);

                return Ok(response.Vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicles for customer {CustomerId}", customerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
        {
            try
            {
                var response = await _vehicleClient.CreateVehicleAsync(
                    request.CustomerId, request.Model, request.VIN, request.LicensePlate,
                    request.Year, request.OdometerKm, request.Status);

                if (!response.Success)
                    return BadRequest(response.Message);

                return CreatedAtAction(nameof(GetVehicle), new { id = response.Vehicle.Id }, response.Vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleRequest request)
        {
            try
            {
                var response = await _vehicleClient.UpdateVehicleAsync(
                    id, request.CustomerId, request.Model, request.VIN, request.LicensePlate,
                    request.Year, request.OdometerKm, request.Status);

                if (!response.Success)
                    return NotFound(response.Message);

                return Ok(response.Vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            try
            {
                var response = await _vehicleClient.DeleteVehicleAsync(id);

                if (!response.Success)
                    return NotFound(response.Message);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    // Request DTOs for Gateway API
    public class CreateVehicleRequest
    {
        public Guid CustomerId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public int OdometerKm { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class UpdateVehicleRequest
    {
        public Guid CustomerId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public int OdometerKm { get; set; }
        public string Status { get; set; } = "Active";
    }
}