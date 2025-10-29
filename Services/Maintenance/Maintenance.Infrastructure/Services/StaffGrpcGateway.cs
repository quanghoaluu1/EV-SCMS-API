using Maintenance.Infrastructure.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Services
{
    public class StaffGrpcGateway : IStaffGrpcGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StaffGrpcGateway> _logger;

        public StaffGrpcGateway(HttpClient httpClient, ILogger<StaffGrpcGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<StaffResponse> GetStaffById(Guid staffId)
        {
            try
            {
                _logger.LogInformation("Calling Staff service to validate staff {StaffId}", staffId);

                var response = await _httpClient.GetAsync($"api/Staff/{staffId}");

                if (response.IsSuccessStatusCode)
                {
                    var staffData = await response.Content.ReadAsStringAsync();
                    
                    _logger.LogInformation("Staff service responded successfully");

                    return new StaffResponse
                    {
                        Success = true,
                        Message = "Staff found",
                        Data = staffData
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Staff not found: {StaffId}", staffId);
                    return new StaffResponse
                    {
                        Success = false,
                        Message = "Staff not found"
                    };
                }
                else
                {
                    _logger.LogError("Staff service returned error status: {StatusCode}", response.StatusCode);
                    return new StaffResponse
                    {
                        Success = false,
                        Message = $"Staff service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Staff service");
                return new StaffResponse
                {
                    Success = false,
                    Message = "Staff service is currently unavailable. Please ensure the Staff service is running."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Staff service");
                return new StaffResponse
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
