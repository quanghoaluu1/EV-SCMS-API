using VehicleService.Grpc;

namespace ApiGateway.Services
{
    public interface IVehicleGrpcClient
    {
        Task<GetVehicleResponse> GetVehicleAsync(Guid id);
        Task<GetAllVehiclesResponse> GetAllVehiclesAsync();
        Task<CreateVehicleResponse> CreateVehicleAsync(Guid customerId, string model, string vin, string licensePlate, int year, int odometerKm, string status);
        Task<UpdateVehicleResponse> UpdateVehicleAsync(Guid id, Guid customerId, string model, string vin, string licensePlate, int year, int odometerKm, string status);
        Task<DeleteVehicleResponse> DeleteVehicleAsync(Guid id);
        Task<GetByCustomerIdResponse> GetByCustomerIdAsync(Guid customerId);
    }

    public class VehicleGrpcClient : IVehicleGrpcClient
    {
        private readonly VehicleService.Grpc.VehicleService.VehicleServiceClient _client;
        private readonly ILogger<VehicleGrpcClient> _logger;

        public VehicleGrpcClient(VehicleService.Grpc.VehicleService.VehicleServiceClient client, ILogger<VehicleGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetVehicleResponse> GetVehicleAsync(Guid id)
        {
            try
            {
                var request = new GetVehicleRequest { Id = id.ToString() };
                return await _client.GetVehicleAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Vehicle Service - GetVehicle for ID: {Id}", id);
                throw;
            }
        }

        public async Task<GetAllVehiclesResponse> GetAllVehiclesAsync()
        {
            try
            {
                var request = new GetAllVehiclesRequest();
                return await _client.GetAllVehiclesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Vehicle Service - GetAllVehicles");
                throw;
            }
        }

        public async Task<CreateVehicleResponse> CreateVehicleAsync(Guid customerId, string model, string vin, string licensePlate, int year, int odometerKm, string status)
        {
            try
            {
                var request = new CreateVehicleRequest
                {
                    CustomerId = customerId.ToString(),
                    Model = model,
                    Vin = vin,
                    LicensePlate = licensePlate,
                    Year = year,
                    OdometerKm = odometerKm,
                    Status = status
                };
                return await _client.CreateVehicleAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Vehicle Service - CreateVehicle");
                throw;
            }
        }

        public async Task<UpdateVehicleResponse> UpdateVehicleAsync(Guid id, Guid customerId, string model, string vin, string licensePlate, int year, int odometerKm, string status)
        {
            try
            {
                var request = new UpdateVehicleRequest
                {
                    Id = id.ToString(),
                    CustomerId = customerId.ToString(),
                    Model = model,
                    Vin = vin,
                    LicensePlate = licensePlate,
                    Year = year,
                    OdometerKm = odometerKm,
                    Status = status
                };
                return await _client.UpdateVehicleAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Vehicle Service - UpdateVehicle for ID: {Id}", id);
                throw;
            }
        }

        public async Task<DeleteVehicleResponse> DeleteVehicleAsync(Guid id)
        {
            try
            {
                var request = new DeleteVehicleRequest { Id = id.ToString() };
                return await _client.DeleteVehicleAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Vehicle Service - DeleteVehicle for ID: {Id}", id);
                throw;
            }
        }

        public async Task<GetByCustomerIdResponse> GetByCustomerIdAsync(Guid customerId)
        {
            try
            {
                var request = new GetByCustomerIdRequest { CustomerId = customerId.ToString() };
                return await _client.GetByCustomerIdAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Vehicle Service - GetByCustomerId for Customer ID: {CustomerId}", customerId);
                throw;
            }
        }
    }
}