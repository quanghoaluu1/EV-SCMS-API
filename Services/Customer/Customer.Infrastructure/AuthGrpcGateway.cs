using Customer.Infrastructure.IRepository;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Customer.Infrastructure;

public class AuthGrpcGateway : IAuthGrpcGateway
{
    private readonly HttpClient _httpClient;
    
    public AuthGrpcGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<ApiResponse<UserData>> GetUserById(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Auth/id?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var userResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserData>>(); 
                Console.WriteLine("response: "  + JsonSerializer.Serialize(userResponse));
                return new ApiResponse<UserData>
                {
                    IsSuccess = userResponse.IsSuccess,
                    Message = userResponse.Message,
                    Data = userResponse.Data
                };
            }
            else
            {
                return new ApiResponse<UserData>
                {
                    IsSuccess = false,
                    Message = $"User not found: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<UserData>
            {
                IsSuccess = false,
                Message = $"Error getting user: {ex.Message}"
            };
        }
    }
}