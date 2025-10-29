using Maintenance.Infrastructure.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Services
{
    public class VehicleGrpcGateway : IVehicleGrpcGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VehicleGrpcGateway> _logger;

        public VehicleGrpcGateway(HttpClient httpClient, ILogger<VehicleGrpcGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<VehicleResponse> GetVehicleById(Guid vehicleId)
        {
            try
            {
                _logger.LogInformation("Calling Vehicle service to validate vehicle {VehicleId}", vehicleId);

                var response = await _httpClient.GetAsync($"api/Vehicle/{vehicleId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    var root = jsonDoc.RootElement;
                    
                    var data = root.TryGetProperty("data", out var dataElement) ? dataElement : root;
                    
                    _logger.LogInformation("Vehicle service responded successfully");

                    return new VehicleResponse
                    {
                        Success = true,
                        Message = "Vehicle found",
                        VehicleId = vehicleId.ToString(),
                        CustomerId = data.TryGetProperty("customerId", out var custId) ? custId.ToString() : string.Empty,
                        Model = data.TryGetProperty("model", out var model) ? model.GetString() ?? string.Empty : string.Empty,
                        LicensePlate = data.TryGetProperty("licensePlate", out var plate) ? plate.GetString() ?? string.Empty : string.Empty
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Vehicle not found: {VehicleId}", vehicleId);
                    return new VehicleResponse
                    {
                        Success = false,
                        Message = "Vehicle not found"
                    };
                }
                else
                {
                    _logger.LogError("Vehicle service returned error status: {StatusCode}", response.StatusCode);
                    return new VehicleResponse
                    {
                        Success = false,
                        Message = $"Vehicle service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Vehicle service");
                return new VehicleResponse
                {
                    Success = false,
                    Message = "Vehicle service is currently unavailable. Please ensure the Vehicle service is running."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Vehicle service");
                return new VehicleResponse
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
