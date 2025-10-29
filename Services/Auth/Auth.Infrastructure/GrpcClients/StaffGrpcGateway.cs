using Auth.Infrastructure.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Auth.Infrastructure.GrpcClients;

public class StaffGrpcGateway : IStaffGrpcGateway
{
    private readonly HttpClient _httpClient;
    
    public StaffGrpcGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<CreateStaffResponse> CreateStaffAsync(Guid userId, DateOnly hireDate, CancellationToken ct = default)
    {
        try
        {
            var requestBody = new
            {
                userId = userId,
                hireDate = hireDate.ToString("yyyy-MM-dd")
            };

            var response = await _httpClient.PostAsJsonAsync("api/Staff", requestBody, ct);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                return new CreateStaffResponse
                {
                    Success = true,
                    Message = "Staff created successfully",
                    StaffId = root.TryGetProperty("staffId", out var staffId) ? staffId.ToString() : string.Empty
                };
            }
            else
            {
                return new CreateStaffResponse
                {
                    Success = false,
                    Message = $"Failed to create staff: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new CreateStaffResponse
            {
                Success = false,
                Message = $"Error creating staff: {ex.Message}"
            };
        }
    }
}