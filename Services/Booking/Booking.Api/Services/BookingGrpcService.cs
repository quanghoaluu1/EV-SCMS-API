using Booking.Application.IServices;
using Booking.Domain.Enums;
using Booking.Infrastructure.IServices;
using BookingGrpc;
using Grpc.Core;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Api.Services
{
    public class BookingGrpcService : BookingGrpc.BookingGrpcService.BookingGrpcServiceBase
    {
        private readonly IBookingService _bookingService;
        private readonly ICustomerGrpcGateway _customerGrpcGateway;
        private readonly IVehicleGrpcGateway _vehicleGrpcGateway;
        private readonly IStaffGrpcGateway _staffGrpcGateway;
        private readonly ILogger<BookingGrpcService> _logger;

        public BookingGrpcService(
            IBookingService bookingService,
            ICustomerGrpcGateway customerGrpcGateway,
            IVehicleGrpcGateway vehicleGrpcGateway,
            IStaffGrpcGateway staffGrpcGateway,
            ILogger<BookingGrpcService> logger)
        {
            _bookingService = bookingService;
            _customerGrpcGateway = customerGrpcGateway;
            _vehicleGrpcGateway = vehicleGrpcGateway;
            _staffGrpcGateway = staffGrpcGateway;
            _logger = logger;
        }

        public override async Task<CreateBookingReply> CreateBooking(CreateBookingRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC CreateBooking called with CustomerId: {CustomerId}, VehicleId: {VehicleId}, StaffId: {StaffId}",
                    request.CustomerId, request.VehicleId, request.StaffId);

                // Validate GUIDs
                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    _logger.LogWarning("Invalid CustomerId format: {CustomerId}", request.CustomerId);
                    return new CreateBookingReply { Success = false, Message = "Invalid CustomerId format" };
                }

                if (!Guid.TryParse(request.VehicleId, out var vehicleId))
                {
                    _logger.LogWarning("Invalid VehicleId format: {VehicleId}", request.VehicleId);
                    return new CreateBookingReply { Success = false, Message = "Invalid VehicleId format" };
                }

                Guid? staffId = null;
                if (!string.IsNullOrEmpty(request.StaffId))
                {
                    if (!Guid.TryParse(request.StaffId, out var parsedStaffId))
                    {
                        _logger.LogWarning("Invalid StaffId format: {StaffId}", request.StaffId);
                        return new CreateBookingReply { Success = false, Message = "Invalid StaffId format" };
                    }
                    staffId = parsedStaffId;
                }

                if (!DateTime.TryParse(request.ScheduledDate, out var scheduledDate))
                {
                    _logger.LogWarning("Invalid ScheduledDate format: {ScheduledDate}", request.ScheduledDate);
                    return new CreateBookingReply { Success = false, Message = "Invalid ScheduledDate format. Use ISO format: yyyy-MM-ddTHH:mm:ssZ" };
                }

                // Validate Customer
                _logger.LogInformation("Validating customer: {CustomerId}", customerId);
                var customerResponse = await _customerGrpcGateway.GetCustomerById(customerId);
                if (!customerResponse.Success)
                {
                    _logger.LogWarning("Customer validation failed: {Message}", customerResponse.Message);
                    return new CreateBookingReply
                    {
                        Success = false,
                        Message = $"Customer validation failed: {customerResponse.Message}"
                    };
                }

                // Validate Vehicle
                _logger.LogInformation("Validating vehicle: {VehicleId}", vehicleId);
                var vehicleResponse = await _vehicleGrpcGateway.GetVehicleById(vehicleId);
                if (!vehicleResponse.Success)
                {
                    _logger.LogWarning("Vehicle validation failed: {Message}", vehicleResponse.Message);
                    return new CreateBookingReply
                    {
                        Success = false,
                        Message = $"Vehicle validation failed: {vehicleResponse.Message}"
                    };
                }

                // Validate Staff (if provided)
                if (staffId.HasValue)
                {
                    _logger.LogInformation("Validating staff: {StaffId}", staffId.Value);
                    var staffResponse = await _staffGrpcGateway.GetStaffById(staffId.Value);
                    if (!staffResponse.Success)
                    {
                        _logger.LogWarning("Staff validation failed: {Message}", staffResponse.Message);
                        return new CreateBookingReply
                        {
                            Success = false,
                            Message = $"Staff validation failed: {staffResponse.Message}"
                        };
                    }
                    _logger.LogInformation("Staff validation successful for: {StaffId}", staffId.Value);
                }

                // Validate service type enum
                if (!Enum.IsDefined(typeof(BookingEnums), request.ServiceType))
                {
                    _logger.LogWarning("Invalid ServiceType: {ServiceType}", request.ServiceType);
                    return new CreateBookingReply { Success = false, Message = "Invalid ServiceType" };
                }

                // Create booking after successful validation
                _logger.LogInformation("All validations passed. Creating booking...");
                var dto = new BookingCreateDto
                {
                    CustomerId = customerId,
                    VehicleId = vehicleId,
                    StaffId = staffId,
                    ServiceType = (BookingEnums)request.ServiceType,
                    ScheduledDate = scheduledDate,
                    Notes = request.Notes
                };

                var booking = await _bookingService.CreateAsync(dto);

                _logger.LogInformation("Booking created successfully with ID: {BookingId}", booking.BookingId);

                return new CreateBookingReply
                {
                    Success = true,
                    Message = "Booking created successfully",
                    BookingId = booking.BookingId.ToString(),
                    CustomerId = booking.CustomerId.ToString(),
                    VehicleId = booking.VehicleId.ToString(),
                    StaffId = booking.StaffId?.ToString() ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateBooking gRPC method");
                return new CreateBookingReply
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }

        public override async Task<GetBookingByIdReply> GetBookingById(GetBookingByIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetBookingById called with BookingId: {BookingId}", request.BookingId);

                if (!Guid.TryParse(request.BookingId, out var bookingId))
                {
                    _logger.LogWarning("Invalid BookingId format: {BookingId}", request.BookingId);
                    return new GetBookingByIdReply { Success = false, Message = "Invalid BookingId format" };
                }

                var booking = await _bookingService.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    _logger.LogInformation("Booking not found: {BookingId}", bookingId);
                    return new GetBookingByIdReply { Success = false, Message = "Booking not found" };
                }

                _logger.LogInformation("Booking found: {BookingId}", bookingId);

                return new GetBookingByIdReply
                {
                    Success = true,
                    Message = "Booking found",
                    BookingId = booking.BookingId.ToString(),
                    CustomerId = booking.CustomerId.ToString(),
                    VehicleId = booking.VehicleId.ToString(),
                    StaffId = booking.StaffId?.ToString() ?? "",
                    ServiceType = (int)booking.ServiceType,
                    ScheduledDate = booking.ScheduledDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = (int)booking.Status,
                    Notes = booking.Notes ?? "",
                    TotalCost = booking.TotalCost ?? 0,
                    PaymentStatus = booking.PaymentStatus,
                    CreatedAt = booking.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    UpdatedAt = booking.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetBookingById gRPC method");
                return new GetBookingByIdReply
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }

        public override async Task<ValidateBookingDataReply> ValidateBookingData(ValidateBookingDataRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC ValidateBookingData called with CustomerId: {CustomerId}, VehicleId: {VehicleId}, StaffId: {StaffId}",
                    request.CustomerId, request.VehicleId, request.StaffId);

                var reply = new ValidateBookingDataReply { Success = true, Message = "Validation completed" };

                // Validate Customer
                if (Guid.TryParse(request.CustomerId, out var customerId))
                {
                    _logger.LogInformation("Validating customer: {CustomerId}", customerId);
                    var customerResponse = await _customerGrpcGateway.GetCustomerById(customerId);
                    reply.CustomerValidation = new ValidationResult
                    {
                        IsValid = customerResponse.Success,
                        Message = customerResponse.Message,
                        EntityData = customerResponse.Success ? $"Customer: {customerResponse.CustomerId}" : ""
                    };
                    _logger.LogInformation("Customer validation result: {IsValid}", customerResponse.Success);
                }
                else
                {
                    _logger.LogWarning("Invalid Customer ID format: {CustomerId}", request.CustomerId);
                    reply.CustomerValidation = new ValidationResult
                    {
                        IsValid = false,
                        Message = "Invalid Customer ID format"
                    };
                }

                // Validate Vehicle
                if (Guid.TryParse(request.VehicleId, out var vehicleId))
                {
                    _logger.LogInformation("Validating vehicle: {VehicleId}", vehicleId);
                    var vehicleResponse = await _vehicleGrpcGateway.GetVehicleById(vehicleId);
                    reply.VehicleValidation = new ValidationResult
                    {
                        IsValid = vehicleResponse.Success,
                        Message = vehicleResponse.Message,
                        EntityData = vehicleResponse.Success ? $"Vehicle: {vehicleResponse.VehicleId}" : ""
                    };
                    _logger.LogInformation("Vehicle validation result: {IsValid}", vehicleResponse.Success);
                }
                else
                {
                    _logger.LogWarning("Invalid Vehicle ID format: {VehicleId}", request.VehicleId);
                    reply.VehicleValidation = new ValidationResult
                    {
                        IsValid = false,
                        Message = "Invalid Vehicle ID format"
                    };
                }

                // Validate Staff (if provided)
                if (!string.IsNullOrEmpty(request.StaffId))
                {
                    if (Guid.TryParse(request.StaffId, out var staffId))
                    {
                        _logger.LogInformation("Validating staff: {StaffId}", staffId);
                        var staffResponse = await _staffGrpcGateway.GetStaffById(staffId);
                        reply.StaffValidation = new ValidationResult
                        {
                            IsValid = staffResponse.Success,
                            Message = staffResponse.Message,
                            EntityData = staffResponse.Success ? $"Staff: {staffResponse.Data?.StaffId}" : ""
                        };
                        _logger.LogInformation("Staff validation result: {IsValid}", staffResponse.Success);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid Staff ID format: {StaffId}", request.StaffId);
                        reply.StaffValidation = new ValidationResult
                        {
                            IsValid = false,
                            Message = "Invalid Staff ID format"
                        };
                    }
                }
                else
                {
                    _logger.LogInformation("Staff ID not provided (optional)");
                    reply.StaffValidation = new ValidationResult
                    {
                        IsValid = true,
                        Message = "Staff ID not provided (optional)"
                    };
                }

                // Overall success depends on all validations
                reply.Success = reply.CustomerValidation.IsValid && reply.VehicleValidation.IsValid && reply.StaffValidation.IsValid;
                if (!reply.Success)
                {
                    reply.Message = "One or more validations failed";
                    _logger.LogWarning("Validation failed - Customer: {Customer}, Vehicle: {Vehicle}, Staff: {Staff}",
                        reply.CustomerValidation.IsValid, reply.VehicleValidation.IsValid, reply.StaffValidation.IsValid);
                }
                else
                {
                    _logger.LogInformation("All validations passed successfully");
                }

                return reply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ValidateBookingData gRPC method");
                return new ValidateBookingDataReply
                {
                    Success = false,
                    Message = $"Validation error: {ex.Message}"
                };
            }
        }

    }
}
