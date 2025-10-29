using Auth.Infrastructure.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Auth.Infrastructure.GrpcClients;

public class CustomerGrpcGateway : ICustomerGrpcGateway
{
    private readonly HttpClient _httpClient;
    
    public CustomerGrpcGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreateCustomerResponse> CreateCustomer(Guid userId, string address)
    {
        try
        {
            var requestBody = new
            {
                userId = userId,
                address = address
            };

            var response = await _httpClient.PostAsJsonAsync("api/Customer", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                return new CreateCustomerResponse
                {
                    Success = true,
                    Message = "Customer created successfully",
                    CustomerId = root.TryGetProperty("id", out var id) ? id.ToString() : string.Empty
                };
            }
            else
            {
                return new CreateCustomerResponse
                {
                    Success = false,
                    Message = $"Failed to create customer: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new CreateCustomerResponse
            {
                Success = false,
                Message = $"Error creating customer: {ex.Message}"
            };
        }
    }
}