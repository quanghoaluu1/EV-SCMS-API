using Booking.Infrastructure.IServices;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleGrpc;

namespace Booking.Infrastructure.Services
{
    public class VehicleGrpcGateway : IVehicleGrpcGateway
    {
        private readonly VehicleGrpcService.VehicleGrpcServiceClient _client;
        private readonly ILogger<VehicleGrpcGateway> _logger;

        public VehicleGrpcGateway(VehicleGrpcService.VehicleGrpcServiceClient client, ILogger<VehicleGrpcGateway> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetVehicleByIdReply> GetVehicleById(Guid vehicleId)
        {
            try
            {
                _logger.LogInformation("Calling Vehicle service to validate vehicle {VehicleId}", vehicleId);

                var request = new GetVehicleByIdRequest
                {
                    VehicleId = vehicleId.ToString()
                };

                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetVehicleByIdAsync(request, callOptions);

                _logger.LogInformation("Vehicle service responded: Success={Success}, Message={Message}",
                    response.Success, response.Message);

                return response;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Vehicle service is unavailable");
                return new GetVehicleByIdReply
                {
                    Success = false,
                    Message = "Vehicle service is currently unavailable. Please ensure the Vehicle service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Vehicle service");
                return new GetVehicleByIdReply
                {
                    Success = false,
                    Message = $"Vehicle service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Vehicle service");
                return new GetVehicleByIdReply
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

    }
}
