using Microsoft.AspNetCore.Mvc;
using Vehicle.Application.DTOs;
using Vehicle.Application.IServices;
using Vehicle.Infrastructure.IService;
using Microsoft.Extensions.Logging;

namespace Vehicle.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICustomerGrpcGateway _customerGrpcGateway;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(IVehicleService vehicleService, ICustomerGrpcGateway customerGrpcGateway, ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _customerGrpcGateway = customerGrpcGateway;
            _logger = logger;
        }

        /// <summary>
        /// Create a new vehicle with customer validation
        /// </summary>
        /// <param name="dto">Vehicle creation data</param>
        /// <returns>Created vehicle or validation error</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating vehicle with customer validation for CustomerId: {CustomerId}", dto.CustomerId);

                // Validate customer exists via gRPC call (same logic as gRPC service)
                var customerResponse = await _customerGrpcGateway.GetCustomerById(dto.CustomerId);
                if (!customerResponse.Success)
                {
                    _logger.LogWarning("Customer validation failed for CustomerId: {CustomerId}. Message: {Message}", 
                        dto.CustomerId, customerResponse.Message);

                    return BadRequest(new { 
                        Success = false, 
                        Message = $"Customer validation failed: {customerResponse.Message}",
                        Error = "InvalidCustomer"
                    });
                }

                _logger.LogInformation("Customer validation successful for CustomerId: {CustomerId}", dto.CustomerId);

                // Create vehicle after successful customer validation
                var result = await _vehicleService.CreateAsync(dto);
                
                _logger.LogInformation("Vehicle created successfully with ID: {VehicleId}", result.Id);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new {
                    Success = true,
                    Message = "Vehicle created successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Create a new vehicle WITHOUT customer validation (for testing)
        /// </summary>
        /// <param name="dto">Vehicle creation data</param>
        /// <returns>Created vehicle</returns>
        [HttpPost("without-validation")]
        public async Task<IActionResult> CreateWithoutValidation([FromBody] VehicleCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating vehicle WITHOUT customer validation for CustomerId: {CustomerId}", dto.CustomerId);

                // Create vehicle directly without customer validation
                var result = await _vehicleService.CreateAsync(dto);
                
                _logger.LogInformation("Vehicle created successfully with ID: {VehicleId}", result.Id);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new {
                    Success = true,
                    Message = "Vehicle created successfully (without customer validation)",
                    Data = result,
                    Warning = "Customer validation was skipped"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle without validation");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get vehicle by ID
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Vehicle details or not found</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _vehicleService.GetByIdAsync(id);
                if (result == null) 
                {
                    return NotFound(new { 
                        Success = false, 
                        Message = "Vehicle not found" 
                    });
                }
                
                return Ok(new {
                    Success = true,
                    Message = "Vehicle found",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle by ID: {VehicleId}", id);
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Get all vehicles for a specific customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of vehicles for the customer</returns>
        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            try
            {
                var result = await _vehicleService.GetByCustomerIdAsync(customerId);
                return Ok(new {
                    Success = true,
                    Message = $"Found {result.Count()} vehicles for customer",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicles for customer: {CustomerId}", customerId);
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Get all vehicles
        /// </summary>
        /// <returns>List of all vehicles</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _vehicleService.GetAllAsync();
                return Ok(new {
                    Success = true,
                    Message = $"Retrieved {result.Count()} vehicles",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all vehicles");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Update an existing vehicle
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <param name="dto">Updated vehicle data</param>
        /// <returns>Updated vehicle or not found</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VehicleUpdateDto dto)
        {
            try
            {
                var result = await _vehicleService.UpdateAsync(id, dto);
                if (result == null) 
                {
                    return NotFound(new { 
                        Success = false, 
                        Message = "Vehicle not found" 
                    });
                }
                
                return Ok(new {
                    Success = true,
                    Message = "Vehicle updated successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle: {VehicleId}", id);
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Delete a vehicle
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Success or not found</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _vehicleService.DeleteAsync(id);
                if (!success) 
                {
                    return NotFound(new { 
                        Success = false, 
                        Message = "Vehicle not found" 
                    });
                }
                
                return Ok(new {
                    Success = true,
                    Message = "Vehicle deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle: {VehicleId}", id);
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Test endpoint to validate a customer ID without creating a vehicle
        /// </summary>
        /// <param name="customerId">Customer ID to validate</param>
        /// <returns>Customer validation result</returns>
        [HttpGet("validate-customer/{customerId:guid}")]
        public async Task<IActionResult> ValidateCustomer(Guid customerId)
        {
            try
            {
                _logger.LogInformation("Testing customer validation for CustomerId: {CustomerId}", customerId);

                var customerResponse = await _customerGrpcGateway.GetCustomerById(customerId);
                
                return Ok(new {
                    Success = customerResponse.Success,
                    Message = customerResponse.Success ? 
                        "Customer exists and is valid" : 
                        $"Customer validation failed: {customerResponse.Message}",
                    CustomerData = customerResponse.Success ? new {
                        CustomerId = customerResponse.CustomerId,
                        UserId = customerResponse.UserId,
                        Address = customerResponse.Address,
                        Status = customerResponse.Status
                    } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating customer: {CustomerId}", customerId);
                return StatusCode(500, new { 
                    Success = false, 
                    Message = $"Error validating customer: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Health check endpoint to test if the service is running
        /// </summary>
        /// <returns>Service health status</returns>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new {
                Success = true,
                Message = "Vehicle service is running",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
