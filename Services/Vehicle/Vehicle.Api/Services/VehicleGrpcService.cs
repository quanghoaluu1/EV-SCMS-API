using Grpc.Core;
using Vehicle.Api;
using Vehicle.Application.DTOs;
using Vehicle.Application.IServices;

namespace VehicleGrpc.API.Services;

public class VehicleGrpcService : Vehicle.Api.Vehicle.VehicleBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleGrpcService(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public override async Task<CreateVehicleReply> CreateVehicle(CreateVehicleRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.CustomerId, out var customerId))
            {
                return new CreateVehicleReply { Success = false, Message = "Invalid CustomerId" };
            }

            // Create vehicle directly - no Auth validation in gRPC service
            var dto = new VehicleCreateDto
            {
                CustomerId = customerId,
                Model = request.Model,
                VIN = request.Vin,
                LicensePlate = request.LicensePlate,
                Year = request.Year,
                OdometerKm = request.OdometerKm,
                Status = request.Status
            };

            var vehicle = await _vehicleService.CreateAsync(dto);

            return new CreateVehicleReply
            {
                Success = true,
                Message = "Vehicle created successfully",
                VehicleId = vehicle.Id.ToString(),
                CustomerId = vehicle.CustomerId.ToString()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vehicle gRPC exception: {ex}");
            return new CreateVehicleReply { Success = false, Message = ex.Message };
        }
    }

    public override async Task<GetVehicleByIdReply> GetVehicleById(GetVehicleByIdRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.VehicleId, out var vehicleId))
            {
                return new GetVehicleByIdReply { Success = false, Message = "Invalid VehicleId" };
            }

            var vehicle = await _vehicleService.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return new GetVehicleByIdReply { Success = false, Message = "Vehicle not found" };
            }

            return new GetVehicleByIdReply
            {
                Success = true,
                Message = "Vehicle found",
                VehicleId = vehicle.Id.ToString(),
                CustomerId = vehicle.CustomerId.ToString(),
                Model = vehicle.Model,
                Vin = vehicle.VIN,
                LicensePlate = vehicle.LicensePlate,
                Year = vehicle.Year,
                OdometerKm = vehicle.OdometerKm,
                Status = vehicle.Status,
                CreatedAt = vehicle.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vehicle gRPC exception: {ex}");
            return new GetVehicleByIdReply { Success = false, Message = ex.Message };
        }
    }

    public override async Task<GetVehiclesByCustomerIdReply> GetVehiclesByCustomerId(GetVehiclesByCustomerIdRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.CustomerId, out var customerId))
            {
                return new GetVehiclesByCustomerIdReply { Success = false, Message = "Invalid CustomerId" };
            }

            var vehicles = await _vehicleService.GetByCustomerIdAsync(customerId);
            var vehicleList = new GetVehiclesByCustomerIdReply
            {
                Success = true,
                Message = "Vehicles found",
                CustomerId = customerId.ToString()
            };

            foreach (var vehicle in vehicles)
            {
                vehicleList.Vehicles.Add(new VehicleInfo
                {
                    VehicleId = vehicle.Id.ToString(),
                    Model = vehicle.Model,
                    Vin = vehicle.VIN,
                    LicensePlate = vehicle.LicensePlate,
                    Year = vehicle.Year,
                    OdometerKm = vehicle.OdometerKm,
                    Status = vehicle.Status,
                    CreatedAt = vehicle.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
            }

            return vehicleList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vehicle gRPC exception: {ex}");
            return new GetVehiclesByCustomerIdReply { Success = false, Message = ex.Message };
        }
    }

    public override async Task<GetAllVehiclesReply> GetAllVehicles(Empty request, ServerCallContext context)
    {
        try
        {
            var vehicles = await _vehicleService.GetAllAsync();
            var vehicleList = new GetAllVehiclesReply
            {
                Success = true,
                Message = "All vehicles retrieved"
            };

            foreach (var vehicle in vehicles)
            {
                vehicleList.Vehicles.Add(new VehicleInfo
                {
                    VehicleId = vehicle.Id.ToString(),
                    CustomerId = vehicle.CustomerId.ToString(),
                    Model = vehicle.Model,
                    Vin = vehicle.VIN,
                    LicensePlate = vehicle.LicensePlate,
                    Year = vehicle.Year,
                    OdometerKm = vehicle.OdometerKm,
                    Status = vehicle.Status,
                    CreatedAt = vehicle.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
            }

            return vehicleList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vehicle gRPC exception: {ex}");
            return new GetAllVehiclesReply { Success = false, Message = ex.Message };
        }
    }

    public override async Task<UpdateVehicleReply> UpdateVehicle(UpdateVehicleRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.VehicleId, out var vehicleId))
            {
                return new UpdateVehicleReply { Success = false, Message = "Invalid VehicleId" };
            }

            var dto = new VehicleUpdateDto
            {
                Model = request.Model,
                VIN = request.Vin,
                LicensePlate = request.LicensePlate,
                Year = request.Year,
                OdometerKm = request.OdometerKm,
                Status = request.Status
            };

            var vehicle = await _vehicleService.UpdateAsync(vehicleId, dto);
            if (vehicle == null)
            {
                return new UpdateVehicleReply { Success = false, Message = "Vehicle not found" };
            }

            return new UpdateVehicleReply
            {
                Success = true,
                Message = "Vehicle updated successfully",
                VehicleId = vehicle.Id.ToString(),
                CustomerId = vehicle.CustomerId.ToString()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vehicle gRPC exception: {ex}");
            return new UpdateVehicleReply { Success = false, Message = ex.Message };
        }
    }

    public override async Task<DeleteVehicleReply> DeleteVehicle(DeleteVehicleRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.VehicleId, out var vehicleId))
            {
                return new DeleteVehicleReply { Success = false, Message = "Invalid VehicleId" };
            }

            var success = await _vehicleService.DeleteAsync(vehicleId);

            return new DeleteVehicleReply
            {
                Success = success,
                Message = success ? "Vehicle deleted successfully" : "Vehicle not found or could not be deleted",
                VehicleId = vehicleId.ToString()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vehicle gRPC exception: {ex}");
            return new DeleteVehicleReply { Success = false, Message = ex.Message };
        }
    }
}