using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vehicle.Infrastructure.IService;
using Microsoft.Extensions.Logging;

namespace Vehicle.Infrastructure.Service
{
    public class CustomerGrpcGateway : ICustomerGrpcGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerGrpcGateway> _logger;

        public CustomerGrpcGateway(HttpClient httpClient, ILogger<CustomerGrpcGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CustomerResponse> GetCustomerById(Guid customerId)
        {
            try
            {
                _logger.LogInformation("Calling Customer service to validate customer {CustomerId}", customerId);

                var response = await _httpClient.GetAsync($"api/Customer/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    var root = jsonDoc.RootElement;
                    
                    _logger.LogInformation("Customer service responded successfully");

                    return new CustomerResponse
                    {
                        Success = true,
                        Message = "Customer found",
                        CustomerId = customerId.ToString(),
                        UserId = root.TryGetProperty("userId", out var userId) ? userId.ToString() : string.Empty,
                        Address = root.TryGetProperty("address", out var address) ? address.GetString() ?? string.Empty : string.Empty,
                        Status = root.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Customer not found: {CustomerId}", customerId);
                    return new CustomerResponse
                    {
                        Success = false,
                        Message = "Customer not found"
                    };
                }
                else
                {
                    _logger.LogError("Customer service returned error status: {StatusCode}", response.StatusCode);
                    return new CustomerResponse
                    {
                        Success = false,
                        Message = $"Customer service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Customer service");
                return new CustomerResponse
                {
                    Success = false,
                    Message = "Customer service is currently unavailable. Please ensure the Customer service is running."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Customer service");
                return new CustomerResponse
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
