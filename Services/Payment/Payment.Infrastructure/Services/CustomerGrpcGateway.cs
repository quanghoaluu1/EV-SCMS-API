using Payment.Infrastructure.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Payment.Infrastructure.Services;

public class CustomerGrpcGateway : ICustomerGrpcGateway
{
    private readonly HttpClient _httpClient;

    public CustomerGrpcGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerResponse> GetCustomerById(Guid customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Customer/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

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
            else
            {
                return new CustomerResponse
                {
                    Success = false,
                    Message = $"Customer not found: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new CustomerResponse
            {
                Success = false,
                Message = $"Error getting customer: {ex.Message}"
            };
        }
    }
}