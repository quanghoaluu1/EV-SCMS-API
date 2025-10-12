using Grpc.Core;
using Maintenance.Application.IServices;
using Maintenance.Domain.Enums;
using Maintenance.Infrastructure.IServices;
using MaintenanceGrpc;
using static Maintenance.Application.DTOs.MaintenanceDTOs;

namespace Maintenance.Api.Services
{
    public class MaintenanceGrpcService : MaintenanceGrpc.MaintenanceGrpcService.MaintenanceGrpcServiceBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ICustomerGrpcGateway _customerGrpcGateway;
        private readonly IVehicleGrpcGateway _vehicleGrpcGateway;
        private readonly IBookingGrpcGateway _bookingGrpcGateway;
        private readonly IStaffGrpcGateway _staffGrpcGateway;
        private readonly ILogger<MaintenanceGrpcService> _logger;

        public MaintenanceGrpcService(
            IMaintenanceService maintenanceService,
            ICustomerGrpcGateway customerGrpcGateway,
            IVehicleGrpcGateway vehicleGrpcGateway,
            IBookingGrpcGateway bookingGrpcGateway,
            IStaffGrpcGateway staffGrpcGateway,
            ILogger<MaintenanceGrpcService> logger)
        {
            _maintenanceService = maintenanceService;
            _customerGrpcGateway = customerGrpcGateway;
            _vehicleGrpcGateway = vehicleGrpcGateway;
            _bookingGrpcGateway = bookingGrpcGateway;
            _staffGrpcGateway = staffGrpcGateway;
            _logger = logger;
        }

        public override async Task<MaintenanceResponse> CreateMaintenance(CreateMaintenanceRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC CreateMaintenance called with VehicleId: {VehicleId}, CustomerId: {CustomerId}, BookingId: {BookingId}, StaffId: {StaffId}",
                    request.VehicleId, request.CustomerId, request.BookingId, request.StaffId);

                // Validate GUIDs
                if (!Guid.TryParse(request.VehicleId, out var vehicleId))
                {
                    _logger.LogWarning("Invalid VehicleId format: {VehicleId}", request.VehicleId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid VehicleId format"));
                }

                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    _logger.LogWarning("Invalid CustomerId format: {CustomerId}", request.CustomerId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CustomerId format"));
                }

                if (!Guid.TryParse(request.BookingId, out var bookingId))
                {
                    _logger.LogWarning("Invalid BookingId format: {BookingId}", request.BookingId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid BookingId format"));
                }

                if (!Guid.TryParse(request.StaffId, out var staffId))
                {
                    _logger.LogWarning("Invalid StaffId format: {StaffId}", request.StaffId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid StaffId format"));
                }

                if (!DateTime.TryParse(request.ServiceDate, out var serviceDate))
                {
                    _logger.LogWarning("Invalid ServiceDate format: {ServiceDate}", request.ServiceDate);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ServiceDate format. Use ISO format: yyyy-MM-ddTHH:mm:ssZ"));
                }

                // Validate Vehicle
                _logger.LogInformation("Validating vehicle: {VehicleId}", vehicleId);
                var vehicleResponse = await _vehicleGrpcGateway.GetVehicleById(vehicleId);
                if (!vehicleResponse.Success)
                {
                    _logger.LogWarning("Vehicle validation failed: {Message}", vehicleResponse.Message);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Vehicle validation failed: {vehicleResponse.Message}"));
                }

                // Validate Customer
                _logger.LogInformation("Validating customer: {CustomerId}", customerId);
                var customerResponse = await _customerGrpcGateway.GetCustomerById(customerId);
                if (!customerResponse.Success)
                {
                    _logger.LogWarning("Customer validation failed: {Message}", customerResponse.Message);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Customer validation failed: {customerResponse.Message}"));
                }

                // Validate Booking
                _logger.LogInformation("Validating booking: {BookingId}", bookingId);
                var bookingResponse = await _bookingGrpcGateway.GetBookingById(bookingId);
                if (!bookingResponse.Success)
                {
                    _logger.LogWarning("Booking validation failed: {Message}", bookingResponse.Message);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Booking validation failed: {bookingResponse.Message}"));
                }

                // Validate Staff
                _logger.LogInformation("Validating staff: {StaffId}", staffId);
                var staffResponse = await _staffGrpcGateway.GetStaffById(staffId);
                if (!staffResponse.Success)
                {
                    _logger.LogWarning("Staff validation failed: {Message}", staffResponse.Message);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Staff validation failed: {staffResponse.Message}"));
                }

                // Validate service type enum
                if (!Enum.IsDefined(typeof(MaintenanceServiceTypeEnum), request.ServiceType))
                {
                    _logger.LogWarning("Invalid ServiceType: {ServiceType}", request.ServiceType);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ServiceType"));
                }

                // Create maintenance after successful validation
                _logger.LogInformation("All validations passed. Creating maintenance...");
                var dto = new CreateMaintenanceDto
                {
                    VehicleId = vehicleId,
                    CustomerId = customerId,
                    BookingId = bookingId,
                    StaffId = staffId,
                    ServiceDate = serviceDate,
                    Odometer = request.Odometer,
                    ServiceType = (MaintenanceServiceTypeEnum)request.ServiceType,
                    Description = request.Description
                };

                var maintenance = await _maintenanceService.CreateAsync(dto);
                _logger.LogInformation("Maintenance created successfully with ID: {MaintenanceId}", maintenance.MaintenanceId);

                return MapToMaintenanceResponse(maintenance);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateMaintenance gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceResponse> GetMaintenanceById(GetMaintenanceByIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenanceById called with MaintenanceId: {MaintenanceId}", request.MaintenanceId);

                if (!Guid.TryParse(request.MaintenanceId, out var maintenanceId))
                {
                    _logger.LogWarning("Invalid MaintenanceId format: {MaintenanceId}", request.MaintenanceId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid MaintenanceId format"));
                }

                var maintenance = await _maintenanceService.GetByIdAsync(maintenanceId);
                if (maintenance == null)
                {
                    _logger.LogInformation("Maintenance not found: {MaintenanceId}", maintenanceId);
                    throw new RpcException(new Status(StatusCode.NotFound, "Maintenance not found"));
                }

                _logger.LogInformation("Maintenance found: {MaintenanceId}", maintenanceId);
                return MapToMaintenanceResponse(maintenance);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenanceById gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceListResponse> GetAllMaintenances(GetAllMaintenancesRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetAllMaintenances called with PageNumber: {PageNumber}, PageSize: {PageSize}",
                    request.PageNumber, request.PageSize);

                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                var maintenances = await _maintenanceService.GetAllAsync(pageNumber, pageSize);
                var maintenanceList = maintenances.ToList();

                _logger.LogInformation("Retrieved {Count} maintenances", maintenanceList.Count);

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllMaintenances gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceResponse> UpdateMaintenance(UpdateMaintenanceRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC UpdateMaintenance called with MaintenanceId: {MaintenanceId}", request.MaintenanceId);

                if (!Guid.TryParse(request.MaintenanceId, out var maintenanceId))
                {
                    _logger.LogWarning("Invalid MaintenanceId format: {MaintenanceId}", request.MaintenanceId);
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid MaintenanceId format"));
                }

                var dto = new UpdateMaintenanceDto();

                // Handle optional fields
                if (!string.IsNullOrEmpty(request.StaffId))
                {
                    if (Guid.TryParse(request.StaffId, out var staffId))
                    {
                        // Validate Staff
                        var staffResponse = await _staffGrpcGateway.GetStaffById(staffId);
                        if (!staffResponse.Success)
                        {
                            throw new RpcException(new Status(StatusCode.NotFound, $"Staff validation failed: {staffResponse.Message}"));
                        }
                        dto.StaffId = staffId;
                    }
                    else
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid StaffId format"));
                    }
                }

                if (!string.IsNullOrEmpty(request.ServiceDate))
                {
                    if (DateTime.TryParse(request.ServiceDate, out var serviceDate))
                    {
                        dto.ServiceDate = serviceDate;
                    }
                    else
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ServiceDate format"));
                    }
                }

                if (request.Odometer > 0)
                {
                    dto.Odometer = request.Odometer;
                }

                if (request.ServiceType > 0 && Enum.IsDefined(typeof(MaintenanceServiceTypeEnum), request.ServiceType))
                {
                    dto.ServiceType = (MaintenanceServiceTypeEnum)request.ServiceType;
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    dto.Description = request.Description;
                }

                if (request.Status > 0 && Enum.IsDefined(typeof(MaintenanceEnums), request.Status))
                {
                    dto.Status = (MaintenanceEnums)request.Status;
                }

                var updated = await _maintenanceService.UpdateAsync(maintenanceId, dto);
                if (updated == null)
                {
                    _logger.LogInformation("Maintenance not found for update: {MaintenanceId}", maintenanceId);
                    throw new RpcException(new Status(StatusCode.NotFound, "Maintenance not found"));
                }

                _logger.LogInformation("Maintenance updated successfully: {MaintenanceId}", maintenanceId);
                return MapToMaintenanceResponse(updated);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateMaintenance gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<DeleteResponse> DeleteMaintenance(DeleteMaintenanceRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC DeleteMaintenance called with MaintenanceId: {MaintenanceId}", request.MaintenanceId);

                if (!Guid.TryParse(request.MaintenanceId, out var maintenanceId))
                {
                    _logger.LogWarning("Invalid MaintenanceId format: {MaintenanceId}", request.MaintenanceId);
                    return new DeleteResponse { Success = false, Message = "Invalid MaintenanceId format" };
                }

                var success = await _maintenanceService.DeleteAsync(maintenanceId);

                if (success)
                {
                    _logger.LogInformation("Maintenance deleted successfully: {MaintenanceId}", maintenanceId);
                    return new DeleteResponse { Success = true, Message = "Maintenance deleted successfully" };
                }
                else
                {
                    _logger.LogInformation("Maintenance not found for deletion: {MaintenanceId}", maintenanceId);
                    return new DeleteResponse { Success = false, Message = "Maintenance not found" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeleteMaintenance gRPC method");
                return new DeleteResponse { Success = false, Message = $"Internal server error: {ex.Message}" };
            }
        }

        public override async Task<MaintenanceListResponse> GetMaintenancesByBookingId(GetMaintenancesByBookingIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenancesByBookingId called with BookingId: {BookingId}", request.BookingId);

                if (!Guid.TryParse(request.BookingId, out var bookingId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid BookingId format"));
                }

                var maintenances = await _maintenanceService.GetByBookingIdAsync(bookingId);
                var maintenanceList = maintenances.ToList();

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenancesByBookingId gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceListResponse> GetMaintenancesByCustomerId(GetMaintenancesByCustomerIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenancesByCustomerId called with CustomerId: {CustomerId}", request.CustomerId);

                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CustomerId format"));
                }

                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                var maintenances = await _maintenanceService.GetByCustomerIdAsync(customerId, pageNumber, pageSize);
                var maintenanceList = maintenances.ToList();

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenancesByCustomerId gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceListResponse> GetMaintenancesByVehicleId(GetMaintenancesByVehicleIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenancesByVehicleId called with VehicleId: {VehicleId}", request.VehicleId);

                if (!Guid.TryParse(request.VehicleId, out var vehicleId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid VehicleId format"));
                }

                var maintenances = await _maintenanceService.GetByVehicleIdAsync(vehicleId);
                var maintenanceList = maintenances.ToList();

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenancesByVehicleId gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceListResponse> GetMaintenancesByStaffId(GetMaintenancesByStaffIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenancesByStaffId called with StaffId: {StaffId}", request.StaffId);

                if (!Guid.TryParse(request.StaffId, out var staffId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid StaffId format"));
                }

                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                var maintenances = await _maintenanceService.GetByStaffIdAsync(staffId, pageNumber, pageSize);
                var maintenanceList = maintenances.ToList();

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenancesByStaffId gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceListResponse> GetMaintenancesByStatus(GetMaintenancesByStatusRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenancesByStatus called with Status: {Status}", request.Status);

                if (!Enum.IsDefined(typeof(MaintenanceEnums), request.Status))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Status"));
                }

                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                var status = (MaintenanceEnums)request.Status;
                var maintenances = await _maintenanceService.GetByStatusAsync(status, pageNumber, pageSize);
                var maintenanceList = maintenances.ToList();

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenancesByStatus gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<MaintenanceListResponse> GetMaintenancesByDateRange(GetMaintenancesByDateRangeRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetMaintenancesByDateRange called with StartDate: {StartDate}, EndDate: {EndDate}",
                    request.StartDate, request.EndDate);

                if (!DateTime.TryParse(request.StartDate, out var startDate))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid StartDate format"));
                }

                if (!DateTime.TryParse(request.EndDate, out var endDate))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid EndDate format"));
                }

                var maintenances = await _maintenanceService.GetByDateRangeAsync(startDate, endDate);
                var maintenanceList = maintenances.ToList();

                var response = new MaintenanceListResponse
                {
                    TotalCount = maintenanceList.Count
                };

                foreach (var maintenance in maintenanceList)
                {
                    response.Maintenances.Add(MapToMaintenanceResponse(maintenance));
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetMaintenancesByDateRange gRPC method");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }

        public override async Task<UpdateResponse> UpdateMaintenanceStatus(UpdateMaintenanceStatusRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC UpdateMaintenanceStatus called with MaintenanceId: {MaintenanceId}, Status: {Status}",
                    request.MaintenanceId, request.Status);

                if (!Guid.TryParse(request.MaintenanceId, out var maintenanceId))
                {
                    return new UpdateResponse { Success = false, Message = "Invalid MaintenanceId format" };
                }

                if (!Enum.IsDefined(typeof(MaintenanceEnums), request.Status))
                {
                    return new UpdateResponse { Success = false, Message = "Invalid Status" };
                }

                var status = (MaintenanceEnums)request.Status;
                var success = await _maintenanceService.UpdateStatusAsync(maintenanceId, status);

                if (success)
                {
                    _logger.LogInformation("Maintenance status updated successfully: {MaintenanceId} to {Status}", maintenanceId, status);
                    return new UpdateResponse { Success = true, Message = "Status updated successfully" };
                }
                else
                {
                    return new UpdateResponse { Success = false, Message = "Maintenance not found" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateMaintenanceStatus gRPC method");
                return new UpdateResponse { Success = false, Message = $"Internal server error: {ex.Message}" };
            }
        }

        public override async Task<UpdateResponse> AssignStaff(AssignStaffRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC AssignStaff called with MaintenanceId: {MaintenanceId}, StaffId: {StaffId}",
                    request.MaintenanceId, request.StaffId);

                if (!Guid.TryParse(request.MaintenanceId, out var maintenanceId))
                {
                    return new UpdateResponse { Success = false, Message = "Invalid MaintenanceId format" };
                }

                if (!Guid.TryParse(request.StaffId, out var staffId))
                {
                    return new UpdateResponse { Success = false, Message = "Invalid StaffId format" };
                }

                // Validate Staff
                var staffResponse = await _staffGrpcGateway.GetStaffById(staffId);
                if (!staffResponse.Success)
                {
                    return new UpdateResponse { Success = false, Message = $"Staff validation failed: {staffResponse.Message}" };
                }

                var success = await _maintenanceService.AssignStaffAsync(maintenanceId, staffId);

                if (success)
                {
                    _logger.LogInformation("Staff assigned successfully to maintenance: {MaintenanceId}", maintenanceId);
                    return new UpdateResponse { Success = true, Message = "Staff assigned successfully" };
                }
                else
                {
                    return new UpdateResponse { Success = false, Message = "Maintenance not found" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AssignStaff gRPC method");
                return new UpdateResponse { Success = false, Message = $"Internal server error: {ex.Message}" };
            }
        }

        #region Private Helper Methods

        private MaintenanceResponse MapToMaintenanceResponse(MaintenanceResponseDto maintenance)
        {
            return new MaintenanceResponse
            {
                MaintenanceId = maintenance.MaintenanceId.ToString(),
                VehicleId = maintenance.VehicleId.ToString(),
                CustomerId = maintenance.CustomerId.ToString(),
                BookingId = maintenance.BookingId.ToString(),
                StaffId = maintenance.StaffId.ToString(),
                ServiceDate = maintenance.ServiceDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Odometer = maintenance.Odometer,
                ServiceType = (int)maintenance.ServiceType,
                ServiceTypeName = maintenance.ServiceTypeName,
                Description = maintenance.Description ?? "",
                Status = (int)maintenance.Status,
                StatusName = maintenance.StatusName,
                CreatedAt = maintenance.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = maintenance.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }

        #endregion

    }
}
