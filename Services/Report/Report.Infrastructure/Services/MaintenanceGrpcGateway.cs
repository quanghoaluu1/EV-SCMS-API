using Grpc.Core;
using MaintenanceGrpc;
using Microsoft.Extensions.Logging;
using Report.Application.IServices;

namespace Report.Infrastructure.Services
{
    public class MaintenanceGrpcGateway : IMaintenanceGrpcGateway
    {
        private readonly MaintenanceGrpcService.MaintenanceGrpcServiceClient _client;
        private readonly ILogger<MaintenanceGrpcGateway> _logger;

        public MaintenanceGrpcGateway(
            MaintenanceGrpcService.MaintenanceGrpcServiceClient client,
            ILogger<MaintenanceGrpcGateway> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<MaintenanceStatsResult> GetMaintenanceStatsAsync()
        {
            try
            {
                _logger.LogInformation("Calling Maintenance service to get maintenance statistics");

                var request = new GetMaintenanceStatsRequest();
                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetMaintenanceStatsAsync(request, callOptions);

                _logger.LogInformation("Maintenance service responded: Success={Success}, Total={Total}",
                    response.Success, response.Total);

                return new MaintenanceStatsResult
                {
                    Success = response.Success,
                    Message = response.Message,
                    Total = response.Total,
                    Pending = response.Pending,
                    InProgress = response.InProgress,
                    Completed = response.Completed,
                    Cancelled = response.Cancelled
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Maintenance service is unavailable");
                return new MaintenanceStatsResult
                {
                    Success = false,
                    Message = "Maintenance service is currently unavailable. Please ensure the Maintenance service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Maintenance service");
                return new MaintenanceStatsResult
                {
                    Success = false,
                    Message = $"Maintenance service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Maintenance service");
                return new MaintenanceStatsResult
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}