using Microsoft.Extensions.Logging;
using Report.Application.IServices;
using System.Net.Http;
using System.Net.Http.Json;

namespace Report.Infrastructure.Services
{
    public class MaintenanceGrpcGateway : IMaintenanceGrpcGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MaintenanceGrpcGateway> _logger;

        public MaintenanceGrpcGateway(
            HttpClient httpClient,
            ILogger<MaintenanceGrpcGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MaintenanceStatsResult> GetMaintenanceStatsAsync()
        {
            try
            {
                _logger.LogInformation("Calling Maintenance service to get maintenance statistics");

                var response = await _httpClient.GetAsync("api/stats/maintenance");

                if (response.IsSuccessStatusCode)
                {
                    var stats = await response.Content.ReadFromJsonAsync<MaintenanceStatsResult>();
                    
                    _logger.LogInformation("Maintenance service responded: Success={Success}, Total={Total}",
                        stats?.Success ?? false, stats?.Total ?? 0);

                    return stats ?? new MaintenanceStatsResult
                    {
                        Success = false,
                        Message = "Failed to parse response"
                    };
                }
                else
                {
                    _logger.LogError("Maintenance service returned error status: {StatusCode}", response.StatusCode);
                    return new MaintenanceStatsResult
                    {
                        Success = false,
                        Message = $"Maintenance service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Maintenance service");
                return new MaintenanceStatsResult
                {
                    Success = false,
                    Message = "Maintenance service is currently unavailable. Please ensure the Maintenance service is running."
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