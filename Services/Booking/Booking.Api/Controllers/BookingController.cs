using Booking.Application.IServices;
using Booking.Domain.Enums;
using Booking.Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ICustomerGrpcGateway _customerGrpcGateway;
        private readonly IVehicleGrpcGateway _vehicleGrpcGateway;
        private readonly IStaffGrpcGateway _staffGrpcGateway;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingService bookingService,
            ICustomerGrpcGateway customerGrpcGateway,
            IVehicleGrpcGateway vehicleGrpcGateway,
            IStaffGrpcGateway staffGrpcGateway,
            ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _customerGrpcGateway = customerGrpcGateway;
            _vehicleGrpcGateway = vehicleGrpcGateway;
            _staffGrpcGateway = staffGrpcGateway;
            _logger = logger;
        }

        /// <summary>
        /// Get all bookings with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>List of bookings</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting all bookings - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid pagination parameters. PageNumber must be >= 1, PageSize must be 1-100",
                        Error = "InvalidPagination"
                    });
                }

                var result = await _bookingService.GetAllAsync(pageNumber, pageSize);

                _logger.LogInformation("Retrieved {Count} bookings", result.Count());

                return Ok(new
                {
                    Success = true,
                    Message = $"Retrieved {result.Count()} bookings",
                    Data = result,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details or not found</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingResponseDto>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting booking by ID: {BookingId}", id);

                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                {
                    _logger.LogInformation("Booking not found: {BookingId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Booking not found",
                        BookingId = id
                    });
                }

                _logger.LogInformation("Booking found: {BookingId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Booking found",
                    Data = booking
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking by ID: {BookingId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Create a new booking with full validation
        /// </summary>
        /// <param name="dto">Booking creation data</param>
        /// <returns>Created booking or validation error</returns>
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] BookingCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating booking with validation for CustomerId: {CustomerId}, VehicleId: {VehicleId}, StaffId: {StaffId}",
                    dto.CustomerId, dto.VehicleId, dto.StaffId);

                // Basic validation
                if (dto.ScheduledDate < DateTime.UtcNow)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Cannot schedule booking in the past",
                        Error = "InvalidScheduledDate"
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

                // Validate Staff (if provided)
                if (dto.StaffId.HasValue)
                {
                    _logger.LogInformation("Validating staff: {StaffId}", dto.StaffId.Value);
                    var staffResponse = await _staffGrpcGateway.GetStaffById(dto.StaffId.Value);
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
                    _logger.LogInformation("Staff validation successful for: {StaffId}", dto.StaffId.Value);
                }

                _logger.LogInformation("All validations successful. Creating booking...");

                // Create booking after successful validation
                var created = await _bookingService.CreateAsync(dto);

                _logger.LogInformation("Booking created successfully with ID: {BookingId}", created.BookingId);

                return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, new
                {
                    Success = true,
                    Message = "Booking created successfully",
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
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Create a new booking WITHOUT validation (for testing)
        /// </summary>
        /// <param name="dto">Booking creation data</param>
        /// <returns>Created booking</returns>
        [HttpPost("without-validation")]
        public async Task<ActionResult<BookingResponseDto>> CreateWithoutValidation([FromBody] BookingCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating booking WITHOUT validation");

                var created = await _bookingService.CreateAsync(dto);

                _logger.LogInformation("Booking created successfully with ID: {BookingId}", created.BookingId);

                return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, new
                {
                    Success = true,
                    Message = "Booking created successfully (without validation)",
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
                _logger.LogError(ex, "Error creating booking without validation");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Update an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="dto">Updated booking data</param>
        /// <returns>Updated booking or not found</returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BookingResponseDto>> Update(Guid id, [FromBody] BookingUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Updating booking: {BookingId}", id);

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

                var updated = await _bookingService.UpdateAsync(id, dto);
                if (updated == null)
                {
                    _logger.LogInformation("Booking not found for update: {BookingId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Booking not found",
                        BookingId = id
                    });
                }

                _logger.LogInformation("Booking updated successfully: {BookingId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Booking updated successfully",
                    Data = updated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking: {BookingId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Delete a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success or not found</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting booking: {BookingId}", id);

                var success = await _bookingService.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogInformation("Booking not found for deletion: {BookingId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Booking not found",
                        BookingId = id
                    });
                }

                _logger.LogInformation("Booking deleted successfully: {BookingId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Booking deleted successfully",
                    BookingId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking: {BookingId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Update booking status
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="newStatus">New booking status</param>
        /// <returns>Success or not found</returns>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] BookingStatusEnum newStatus)
        {
            try
            {
                _logger.LogInformation("Updating booking status: {BookingId} to {Status}", id, newStatus);

                if (!Enum.IsDefined(typeof(BookingStatusEnum), newStatus))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid booking status",
                        Error = "InvalidStatus"
                    });
                }

                var success = await _bookingService.UpdateStatusAsync(id, newStatus);
                if (!success)
                {
                    _logger.LogInformation("Booking not found for status update: {BookingId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Booking not found",
                        BookingId = id
                    });
                }

                _logger.LogInformation("Booking status updated successfully: {BookingId} to {Status}", id, newStatus);

                return Ok(new
                {
                    Success = true,
                    Message = "Booking status updated successfully",
                    BookingId = id,
                    NewStatus = newStatus.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status: {BookingId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Assign staff to a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="staffId">Staff ID to assign</param>
        /// <returns>Success or not found</returns>
        [HttpPatch("{id:guid}/assign/{staffId:guid}")]
        public async Task<IActionResult> AssignStaff(Guid id, Guid staffId)
        {
            try
            {
                _logger.LogInformation("Assigning staff {StaffId} to booking: {BookingId}", staffId, id);

                // Validate Staff before assignment
                var staffResponse = await _staffGrpcGateway.GetStaffById(staffId);
                if (!staffResponse.Success)
                {
                    _logger.LogWarning("Staff validation failed for assignment. StaffId: {StaffId}. Message: {Message}",
                        staffId, staffResponse.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"Staff validation failed: {staffResponse.Message}",
                        Error = "InvalidStaff",
                        StaffId = staffId
                    });
                }

                var success = await _bookingService.AssignStaffAsync(id, staffId);
                if (!success)
                {
                    _logger.LogInformation("Booking not found for staff assignment: {BookingId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Booking not found",
                        BookingId = id
                    });
                }

                _logger.LogInformation("Staff assigned successfully to booking: {BookingId}", id);

                return Ok(new
                {
                    Success = true,
                    Message = "Staff assigned successfully",
                    BookingId = id,
                    StaffId = staffId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning staff to booking: {BookingId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Test endpoint to validate booking data without creating a booking
        /// </summary>
        /// <param name="customerId">Customer ID to validate</param>
        /// <param name="vehicleId">Vehicle ID to validate</param>
        /// <param name="staffId">Staff ID to validate (optional)</param>
        /// <returns>Validation results for all entities</returns>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateBookingData(
            [FromQuery] Guid customerId,
            [FromQuery] Guid vehicleId,
            [FromQuery] Guid? staffId = null)
        {
            try
            {
                _logger.LogInformation("Validating booking data - CustomerId: {CustomerId}, VehicleId: {VehicleId}, StaffId: {StaffId}",
                    customerId, vehicleId, staffId);

                var validationResults = new
                {
                    CustomerValidation = await ValidateCustomer(customerId),
                    VehicleValidation = await ValidateVehicle(vehicleId),
                    StaffValidation = staffId.HasValue ? await ValidateStaff(staffId.Value) : new { IsValid = true, Message = "Staff ID not provided (optional)", Data = (object?)null }
                };

                var allValid = validationResults.CustomerValidation.IsValid &&
                              validationResults.VehicleValidation.IsValid &&
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
                _logger.LogError(ex, "Error validating booking data");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error validating booking data: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get bookings by customer ID
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>List of bookings for the customer</returns>
        [HttpGet("customer/{customerId:guid}")]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> GetByCustomerId(
            Guid customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting bookings for customer: {CustomerId}", customerId);

                var result = await _bookingService.GetByCustomerIdAsync(customerId, pageNumber, pageSize);

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {result.Count()} bookings for customer",
                    Data = result,
                    CustomerId = customerId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for customer: {CustomerId}", customerId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Get bookings by date
        /// </summary>
        /// <param name="date">Date to filter bookings</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 100)</param>
        /// <returns>List of bookings for the specified date</returns>
        [HttpGet("date/{date:datetime}")]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> GetByDate(
            DateTime date,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                _logger.LogInformation("Getting bookings for date: {Date}", date.ToString("yyyy-MM-dd"));

                var result = await _bookingService.GetBookingsByDateAsync(date, pageNumber, pageSize);

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {result.Count()} bookings for {date:yyyy-MM-dd}",
                    Data = result,
                    Date = date.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for date: {Date}", date);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Error = "ServerError"
                });
            }
        }

        /// <summary>
        /// Update payment status
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="paymentStatus">New payment status</param>
        /// <returns>Success or not found</returns>
        [HttpPatch("{id:guid}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] string paymentStatus)
        {
            try
            {
                _logger.LogInformation("Updating payment status for booking: {BookingId} to {PaymentStatus}", id, paymentStatus);

                var success = await _bookingService.UpdatePaymentStatusAsync(id, paymentStatus);
                if (!success)
                {
                    _logger.LogInformation("Booking not found for payment status update: {BookingId}", id);
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Booking not found",
                        BookingId = id
                    });
                }

                _logger.LogInformation("Payment status updated successfully: {BookingId} to {PaymentStatus}", id, paymentStatus);

                return Ok(new
                {
                    Success = true,
                    Message = "Payment status updated successfully",
                    BookingId = id,
                    PaymentStatus = paymentStatus
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid payment status: {PaymentStatus}", paymentStatus);
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = "InvalidPaymentStatus"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status: {BookingId}", id);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
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
                Message = "Booking service is running",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        #region Private Helper Methods

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
