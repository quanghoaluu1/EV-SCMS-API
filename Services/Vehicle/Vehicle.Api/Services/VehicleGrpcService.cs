using Grpc.Core;
using Vehicle.Application.DTOs;
using Vehicle.Application.IServices;
using VehicleService.Grpc;

namespace Vehicle.Api.Services
{
    public class VehicleGrpcService : VehicleService.Grpc.VehicleService.VehicleServiceBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleGrpcService> _logger;

        public VehicleGrpcService(
            IVehicleService vehicleService,
            ILogger<VehicleGrpcService> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        public override async Task<GetVehicleResponse> GetVehicle(
            GetVehicleRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id))
                {
                    return new GetVehicleResponse
                    {
                        Success = false,
                        Message = "Invalid ID format"
                    };
                }

                var vehicle = await _vehicleService.GetByIdAsync(id);

                if (vehicle == null)
                {
                    return new GetVehicleResponse
                    {
                        Success = false,
                        Message = $"Vehicle with ID {request.Id} not found"
                    };
                }

                return new GetVehicleResponse
                {
                    Vehicle = MapToGrpcVehicle(vehicle),
                    Success = true,
                    Message = "Vehicle retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle with ID: {Id}", request.Id);
                return new GetVehicleResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<CreateVehicleResponse> CreateVehicle(
            CreateVehicleRequest request,
            ServerCallContext context)
        {
            try
            {
                var createDto = new VehicleCreateDto
                {
                    CustomerId = Guid.Parse(request.CustomerId),
                    Model = request.Model,
                    VIN = request.Vin,
                    LicensePlate = request.LicensePlate,
                    Year = request.Year,
                    OdometerKm = request.OdometerKm,
                    Status = request.Status
                };

                var createdVehicle = await _vehicleService.CreateAsync(createDto);

                return new CreateVehicleResponse
                {
                    Vehicle = MapToGrpcVehicle(createdVehicle),
                    Success = true,
                    Message = "Vehicle created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle");
                return new CreateVehicleResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<UpdateVehicleResponse> UpdateVehicle(
            UpdateVehicleRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id))
                {
                    return new UpdateVehicleResponse
                    {
                        Success = false,
                        Message = "Invalid ID format"
                    };
                }

                var updateDto = new VehicleUpdateDto
                {
                    CustomerId = Guid.Parse(request.CustomerId),
                    Model = request.Model,
                    VIN = request.Vin,
                    LicensePlate = request.LicensePlate,
                    Year = request.Year,
                    OdometerKm = request.OdometerKm,
                    Status = request.Status
                };

                var updatedVehicle = await _vehicleService.UpdateAsync(id, updateDto);

                if (updatedVehicle == null)
                {
                    return new UpdateVehicleResponse
                    {
                        Success = false,
                        Message = $"Vehicle with ID {request.Id} not found"
                    };
                }

                return new UpdateVehicleResponse
                {
                    Vehicle = MapToGrpcVehicle(updatedVehicle),
                    Success = true,
                    Message = "Vehicle updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle with ID: {Id}", request.Id);
                return new UpdateVehicleResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<DeleteVehicleResponse> DeleteVehicle(
            DeleteVehicleRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id))
                {
                    return new DeleteVehicleResponse
                    {
                        Success = false,
                        Message = "Invalid ID format"
                    };
                }

                var success = await _vehicleService.DeleteAsync(id);

                return new DeleteVehicleResponse
                {
                    Success = success,
                    Message = success ? "Vehicle deleted successfully" : $"Vehicle with ID {request.Id} not found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle with ID: {Id}", request.Id);
                return new DeleteVehicleResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<GetAllVehiclesResponse> GetAllVehicles(
            GetAllVehiclesRequest request,
            ServerCallContext context)
        {
            try
            {
                var vehicles = await _vehicleService.GetAllAsync();

                var response = new GetAllVehiclesResponse
                {
                    Success = true,
                    Message = "Vehicles retrieved successfully"
                };

                response.Vehicles.AddRange(vehicles.Select(MapToGrpcVehicle));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all vehicles");
                return new GetAllVehiclesResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        public override async Task<GetByCustomerIdResponse> GetByCustomerId(
            GetByCustomerIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.CustomerId, out var customerId))
                {
                    return new GetByCustomerIdResponse
                    {
                        Success = false,
                        Message = "Invalid Customer ID format"
                    };
                }

                var vehicles = await _vehicleService.GetByCustomerIdAsync(customerId);

                var response = new GetByCustomerIdResponse
                {
                    Success = true,
                    Message = "Vehicles retrieved successfully"
                };

                response.Vehicles.AddRange(vehicles.Select(MapToGrpcVehicle));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicles by customer ID: {CustomerId}", request.CustomerId);
                return new GetByCustomerIdResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        private static VehicleService.Grpc.Vehicle MapToGrpcVehicle(VehicleDto vehicle)
        {
            return new VehicleService.Grpc.Vehicle
            {
                Id = vehicle.Id.ToString(),
                CustomerId = vehicle.CustomerId.ToString(),
                Model = vehicle.Model,
                Vin = vehicle.VIN,
                LicensePlate = vehicle.LicensePlate,
                Year = vehicle.Year,
                OdometerKm = vehicle.OdometerKm,
                Status = vehicle.Status,
                CreatedAt = vehicle.CreatedAt.Ticks,
                UpdatedAt = vehicle.UpdatedAt.Ticks
            };
        }
    }
}
