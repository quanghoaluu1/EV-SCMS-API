using Maintenance.Application.IServices;
using Maintenance.Domain.Enums;
using Maintenance.Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using static Maintenance.Application.DTOs.MaintenanceDTOs;

namespace Maintenance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ICustomerGrpcGateway _customerGrpcGateway;
        private readonly IVehicleGrpcGateway _vehicleGrpcGateway;
        private readonly IBookingGrpcGateway _bookingGrpcGateway;
        private readonly IStaffGrpcGateway _staffGrpcGateway;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(
            IMaintenanceService maintenanceService,
            ICustomerGrpcGateway customerGrpcGateway,
            IVehicleGrpcGateway vehicleGrpcGateway,
            IBookingGrpcGateway bookingGrpcGateway,
            IStaffGrpcGateway staffGrpcGateway,
            ILogger<MaintenanceController> logger)
        {
            _maintenanceService = maintenanceService;
            _customerGrpcGateway = customerGrpcGateway;
            _vehicleGrpcGateway = vehicleGrpcGateway;
            _bookingGrpcGateway = bookingGrpcGateway;
            _staffGrpcGateway = staffGrpcGateway;
            _logger = logger;
        }

        /// <summary>
        /// Get all maintenances with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>List of maintenances</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting all maintenances - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid pagination parameters. PageNumber must be >= 1, PageSize must be 1-100",
                        Error = "InvalidPagination"
                    });
                }

                var result = await _maintenanceService.GetAllAsync(pageNumber, pageSize);
                var maintenanceList = result.ToList();

                _logger.LogInformation("Retrieved {Count} maintenances", maintenanceList.Count);

                return Ok(new
                {
                    Success = true,
                    Message = $"Retrieved {maintenanceList.Count} maintenances",
                    Data = maintenanceList,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenances");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenance by ID
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <returns>Maintenance details or not found</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MaintenanceResponseDto>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting maintenance by ID: {MaintenanceId}", id);

                var maintenance = await _maintenanceService.GetByIdAsync(id);
                if (maintenance == null)
                {
                    _logger.LogInformation("Maintenance not found: {MaintenanceId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Maintenance not found",
                        MaintenanceId = id
                    });
                }

                _logger.LogInformation("Maintenance found: {MaintenanceId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Maintenance found",
                    Data = maintenance
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance by ID: {MaintenanceId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Create a new maintenance with full validation
        /// </summary>
        /// <param name="dto">Maintenance creation data</param>
        /// <returns>Created maintenance or validation error</returns>
        [HttpPost]
        public async Task<ActionResult<MaintenanceResponseDto>> Create([FromBody] CreateMaintenanceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid input data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                        Error = "ValidationFailed"
                    });
                }

                _logger.LogInformation("Creating maintenance with validation for VehicleId: {VehicleId}, CustomerId: {CustomerId}, BookingId: {BookingId}, StaffId: {StaffId}",
                    dto.VehicleId, dto.CustomerId, dto.BookingId, dto.StaffId);

                // Basic validation
                if (dto.ServiceDate < DateTime.UtcNow.AddHours(-1)) // Allow 1 hour buffer for timezone issues
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Cannot schedule maintenance in the past",
                        Error = "InvalidServiceDate"
                    });
                }

                // Validate Vehicle
                _logger.LogInformation("Validating vehicle: {VehicleId}", dto.VehicleId);
                var vehicleResponse = await _vehicleGrpcGateway.GetVehicleById(dto.VehicleId);
                if (!vehicleResponse.Success)
                {
                    _logger.LogWarning("Vehicle validation failed for VehicleId: {VehicleId}. Message: {Message}",
                        dto.VehicleId, vehicleResponse.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Vehicle validation failed: {vehicleResponse.Message}",
                        Error = "InvalidVehicle",
                        VehicleId = dto.VehicleId
                    });
                }

                // Validate Customer
                _logger.LogInformation("Validating customer: {CustomerId}", dto.CustomerId);
                var customerResponse = await _customerGrpcGateway.GetCustomerById(dto.CustomerId);
                if (!customerResponse.Success)
                {
                    _logger.LogWarning("Customer validation failed for CustomerId: {CustomerId}. Message: {Message}",
                        dto.CustomerId, customerResponse.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Customer validation failed: {customerResponse.Message}",
                        Error = "InvalidCustomer",
                        CustomerId = dto.CustomerId
                    });
                }

                // Validate Booking
                _logger.LogInformation("Validating booking: {BookingId}", dto.BookingId);
                var bookingResponse = await _bookingGrpcGateway.GetBookingById(dto.BookingId);
                if (!bookingResponse.Success)
                {
                    _logger.LogWarning("Booking validation failed for BookingId: {BookingId}. Message: {Message}",
                        dto.BookingId, bookingResponse.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Booking validation failed: {bookingResponse.Message}",
                        Error = "InvalidBooking",
                        BookingId = dto.BookingId
                    });
                }

                // Validate Staff
                _logger.LogInformation("Validating staff: {StaffId}", dto.StaffId);
                var staffResponse = await _staffGrpcGateway.GetStaffById(dto.StaffId);
                if (!staffResponse.Success)
                {
                    _logger.LogWarning("Staff validation failed for StaffId: {StaffId}. Message: {Message}",
                        dto.StaffId, staffResponse.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Staff validation failed: {staffResponse.Message}",
                        Error = "InvalidStaff",
                        StaffId = dto.StaffId
                    });
                }

                _logger.LogInformation("All validations successful. Creating maintenance...");

                // Create maintenance after successful validation
                var created = await _maintenanceService.CreateAsync(dto);

                _logger.LogInformation("Maintenance created successfully with ID: {MaintenanceId}", created.MaintenanceId);

                return CreatedAtAction(nameof(GetById), new { id = created.MaintenanceId }, new
                {
                    Success = true,
                    Message = "Maintenance created successfully",
                    Data = created
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Business rule validation failed");
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = "BusinessRuleViolation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Create a new maintenance WITHOUT validation (for testing)
        /// </summary>
        /// <param name="dto">Maintenance creation data</param>
        /// <returns>Created maintenance</returns>
        [HttpPost("without-validation")]
        public async Task<ActionResult<MaintenanceResponseDto>> CreateWithoutValidation([FromBody] CreateMaintenanceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating maintenance WITHOUT validation");

                var created = await _maintenanceService.CreateAsync(dto);

                _logger.LogInformation("Maintenance created successfully with ID: {MaintenanceId}", created.MaintenanceId);

                return CreatedAtAction(nameof(GetById), new { id = created.MaintenanceId }, new
                {
                    Success = true,
                    Message = "Maintenance created successfully (without validation)",
                    Data = created,
                    Warning = "Entity validation was skipped"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Business rule validation failed");
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = "BusinessRuleViolation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance without validation");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Update an existing maintenance
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <param name="dto">Updated maintenance data</param>
        /// <returns>Updated maintenance or not found</returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MaintenanceResponseDto>> Update(Guid id, [FromBody] UpdateMaintenanceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid input data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                        Error = "ValidationFailed"
                    });
                }

                _logger.LogInformation("Updating maintenance: {MaintenanceId}", id);

                // Validate Staff (if provided in update)
                if (dto.StaffId.HasValue)
                {
                    _logger.LogInformation("Validating staff for update: {StaffId}", dto.StaffId.Value);
                    var staffResponse = await _staffGrpcGateway.GetStaffById(dto.StaffId.Value);
                    if (!staffResponse.Success)
                    {
                        _logger.LogWarning("Staff validation failed during update for StaffId: {StaffId}. Message: {Message}",
                            dto.StaffId, staffResponse.Message);

                        return BadRequest(new
                        {
                            Success = false,
                            Message = $"Staff validation failed: {staffResponse.Message}",
                            Error = "InvalidStaff",
                            StaffId = dto.StaffId
                        });
                    }
                }

                var updated = await _maintenanceService.UpdateAsync(id, dto);
                if (updated == null)
                {
                    _logger.LogInformation("Maintenance not found for update: {MaintenanceId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Maintenance not found",
                        MaintenanceId = id
                    });
                }

                _logger.LogInformation("Maintenance updated successfully: {MaintenanceId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Maintenance updated successfully",
                    Data = updated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance: {MaintenanceId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Delete a maintenance
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <returns>Success or not found</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting maintenance: {MaintenanceId}", id);

                var success = await _maintenanceService.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogInformation("Maintenance not found for deletion: {MaintenanceId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Maintenance not found",
                        MaintenanceId = id
                    });
                }

                _logger.LogInformation("Maintenance deleted successfully: {MaintenanceId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Maintenance deleted successfully",
                    MaintenanceId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance: {MaintenanceId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenances by booking ID
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <returns>List of maintenances for the booking</returns>
        [HttpGet("booking/{bookingId:guid}")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetByBookingId(Guid bookingId)
        {
            try
            {
                _logger.LogInformation("Getting maintenances for booking: {BookingId}", bookingId);

                var result = await _maintenanceService.GetByBookingIdAsync(bookingId);
                var maintenanceList = result.ToList();

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {maintenanceList.Count} maintenances for booking",
                    Data = maintenanceList,
                    BookingId = bookingId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for booking: {BookingId}", bookingId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenances by customer ID
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>List of maintenances for the customer</returns>
        [HttpGet("customer/{customerId:guid}")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetByCustomerId(
            Guid customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting maintenances for customer: {CustomerId}", customerId);

                var result = await _maintenanceService.GetByCustomerIdAsync(customerId, pageNumber, pageSize);
                var maintenanceList = result.ToList();

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {maintenanceList.Count} maintenances for customer",
                    Data = maintenanceList,
                    CustomerId = customerId,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for customer: {CustomerId}", customerId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenances by vehicle ID
        /// </summary>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <returns>List of maintenances for the vehicle</returns>
        [HttpGet("vehicle/{vehicleId:guid}")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetByVehicleId(Guid vehicleId)
        {
            try
            {
                _logger.LogInformation("Getting maintenances for vehicle: {VehicleId}", vehicleId);

                var result = await _maintenanceService.GetByVehicleIdAsync(vehicleId);
                var maintenanceList = result.ToList();

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {maintenanceList.Count} maintenances for vehicle",
                    Data = maintenanceList,
                    VehicleId = vehicleId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for vehicle: {VehicleId}", vehicleId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenances by staff ID
        /// </summary>
        /// <param name="staffId">Staff ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>List of maintenances for the staff</returns>
        [HttpGet("staff/{staffId:guid}")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetByStaffId(
            Guid staffId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting maintenances for staff: {StaffId}", staffId);

                var result = await _maintenanceService.GetByStaffIdAsync(staffId, pageNumber, pageSize);
                var maintenanceList = result.ToList();

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {maintenanceList.Count} maintenances for staff",
                    Data = maintenanceList,
                    StaffId = staffId,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for staff: {StaffId}", staffId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenances by status
        /// </summary>
        /// <param name="status">Maintenance status</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>List of maintenances with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetByStatus(
            MaintenanceEnums status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting maintenances by status: {Status}", status);

                var result = await _maintenanceService.GetByStatusAsync(status, pageNumber, pageSize);
                var maintenanceList = result.ToList();

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {maintenanceList.Count} maintenances with status {status}",
                    Data = maintenanceList,
                    Status = status.ToString(),
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances by status: {Status}", status);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get maintenances by date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of maintenances in the specified date range</returns>
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting maintenances by date range: {StartDate} to {EndDate}",
                    startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

                if (startDate > endDate)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Start date must be before end date",
                        Error = "InvalidDateRange"
                    });
                }

                var result = await _maintenanceService.GetByDateRangeAsync(startDate, endDate);
                var maintenanceList = result.ToList();

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {maintenanceList.Count} maintenances in date range",
                    Data = maintenanceList,
                    DateRange = new
                    {
                        StartDate = startDate.ToString("yyyy-MM-dd"),
                        EndDate = endDate.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances by date range");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Update maintenance status
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <param name="dto">Status update data</param>
        /// <returns>Success or not found</returns>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                _logger.LogInformation("Updating maintenance status: {MaintenanceId} to {Status}", id, dto.Status);

                var success = await _maintenanceService.UpdateStatusAsync(id, dto.Status);
                if (!success)
                {
                    _logger.LogInformation("Maintenance not found for status update: {MaintenanceId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Maintenance not found",
                        MaintenanceId = id
                    });
                }

                _logger.LogInformation("Maintenance status updated successfully: {MaintenanceId} to {Status}", id, dto.Status);

                return Ok(new
                {
                    Success = true,
                    Message = "Maintenance status updated successfully",
                    MaintenanceId = id,
                    NewStatus = dto.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance status: {MaintenanceId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Assign staff to a maintenance
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <param name="dto">Staff assignment data</param>
        /// <returns>Success or not found</returns>
        [HttpPatch("{id:guid}/assign-staff")]
        public async Task<IActionResult> AssignStaff(Guid id, [FromBody] AssignStaffDto dto)
        {
            try
            {
                _logger.LogInformation("Assigning staff {StaffId} to maintenance: {MaintenanceId}", dto.StaffId, id);

                // Validate Staff before assignment
                var staffResponse = await _staffGrpcGateway.GetStaffById(dto.StaffId);
                if (!staffResponse.Success)
                {
                    _logger.LogWarning("Staff validation failed for assignment. StaffId: {StaffId}. Message: {Message}",
                        dto.StaffId, staffResponse.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Staff validation failed: {staffResponse.Message}",
                        Error = "InvalidStaff",
                        StaffId = dto.StaffId
                    });
                }

                var success = await _maintenanceService.AssignStaffAsync(id, dto.StaffId);
                if (!success)
                {
                    _logger.LogInformation("Maintenance not found for staff assignment: {MaintenanceId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Maintenance not found",
                        MaintenanceId = id
                    });
                }

                _logger.LogInformation("Staff assigned successfully to maintenance: {MaintenanceId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Staff assigned successfully",
                    MaintenanceId = id,
                    StaffId = dto.StaffId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning staff to maintenance: {MaintenanceId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Validate maintenance data without creating maintenance
        /// </summary>
        /// <param name="vehicleId">Vehicle ID to validate</param>
        /// <param name="customerId">Customer ID to validate</param>
        /// <param name="bookingId">Booking ID to validate</param>
        /// <param name="staffId">Staff ID to validate</param>
        /// <returns>Validation results for all entities</returns>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateMaintenanceData(
            [FromQuery] Guid vehicleId,
            [FromQuery] Guid customerId,
            [FromQuery] Guid bookingId,
            [FromQuery] Guid staffId)
        {
            try
            {
                _logger.LogInformation("Validating maintenance data - VehicleId: {VehicleId}, CustomerId: {CustomerId}, BookingId: {BookingId}, StaffId: {StaffId}",
                    vehicleId, customerId, bookingId, staffId);

                var validationResults = new
                {
                    VehicleValidation = await ValidateVehicle(vehicleId),
                    CustomerValidation = await ValidateCustomer(customerId),
                    BookingValidation = await ValidateBooking(bookingId),
                    StaffValidation = await ValidateStaff(staffId)
                };

                var allValid = validationResults.VehicleValidation.IsValid &&
                              validationResults.CustomerValidation.IsValid &&
                              validationResults.BookingValidation.IsValid &&
                              validationResults.StaffValidation.IsValid;

                return Ok(new
                {
                    Success = allValid,
                    Message = allValid ? "All validations passed" : "One or more validations failed",
                    ValidationResults = validationResults
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating maintenance data");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error validating maintenance data: {ex.Message}",
                    Error = "ServerError"
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
            return Ok(new
            {
                Success = true,
                Message = "Maintenance service is running",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        #region Private Helper Methods

        private async Task<dynamic> ValidateVehicle(Guid vehicleId)
        {
            var vehicleResponse = await _vehicleGrpcGateway.GetVehicleById(vehicleId);
            return new
            {
                IsValid = vehicleResponse.Success,
                Message = vehicleResponse.Message,
                Data = vehicleResponse.Success ? new
                {
                    VehicleId = vehicleResponse.VehicleId,
                    CustomerId = vehicleResponse.CustomerId,
                    Model = vehicleResponse.Model,
                    LicensePlate = vehicleResponse.LicensePlate
                } : null
            };
        }

        private async Task<dynamic> ValidateCustomer(Guid customerId)
        {
            var customerResponse = await _customerGrpcGateway.GetCustomerById(customerId);
            return new
            {
                IsValid = customerResponse.Success,
                Message = customerResponse.Message,
                Data = customerResponse.Success ? new
                {
                    CustomerId = customerResponse.CustomerId,
                    UserId = customerResponse.UserId,
                    Address = customerResponse.Address,
                    Status = customerResponse.Status
                } : null
            };
        }

        private async Task<dynamic> ValidateBooking(Guid bookingId)
        {
            var bookingResponse = await _bookingGrpcGateway.GetBookingById(bookingId);
            return new
            {
                IsValid = bookingResponse.Success,
                Message = bookingResponse.Message,
                Data = bookingResponse.Success ? new
                {
                    BookingId = bookingResponse.BookingId,
                    CustomerId = bookingResponse.CustomerId,
                    VehicleId = bookingResponse.VehicleId,
                    Status = bookingResponse.Status
                } : null
            };
        }

        private async Task<dynamic> ValidateStaff(Guid staffId)
        {
            var staffResponse = await _staffGrpcGateway.GetStaffById(staffId);
            return new
            {
                IsValid = staffResponse.Success,
                Message = staffResponse.Message,
                Data = staffResponse.Success ? staffResponse.Data : null
            };
        }

        #endregion

    }
}
