using Infrastructure.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services;

public class AuthGrpcGateway : IAuthGrpcGateway
{
    private readonly HttpClient _httpClient;
    
    public AuthGrpcGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<GetUserByIdResponse> GetUserById(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Auth/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                return new GetUserByIdResponse
                {
                    Success = true,
                    Message = "User found",
                    UserId = root.TryGetProperty("userId", out var uid) ? uid.ToString() : string.Empty,
                    Email = root.TryGetProperty("email", out var email) ? email.GetString() ?? string.Empty : string.Empty,
                    Role = root.TryGetProperty("role", out var role) ? role.GetString() ?? string.Empty : string.Empty
                };
            }
            else
            {
                return new GetUserByIdResponse
                {
                    Success = false,
                    Message = $"User not found: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new GetUserByIdResponse
            {
                Success = false,
                Message = $"Error getting user: {ex.Message}"
            };
        }
    }
}