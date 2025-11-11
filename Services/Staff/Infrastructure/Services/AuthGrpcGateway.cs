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
    
    public async Task<ApiResponse<UserData>> GetUserById(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Auth/id?userId={userId}");
            var userResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserData>>();
            Console.WriteLine("userResponse:" + JsonSerializer.Serialize(userResponse));
            
            if (userResponse is { IsSuccess: true, Data: not null })
            {
                return new ApiResponse<UserData>
                {
                    IsSuccess = true,
                    Message = "User found",
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

    public async Task<ApiResponse<List<UserData>>> GetUserByRole(int role)
    {
        var response = await _httpClient.GetAsync($"api/Auth/role?role={role}");
        var userResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserData>>>();

        Console.WriteLine("userResponse:" + JsonSerializer.Serialize(userResponse));

        if (userResponse is { IsSuccess: true, Data: not null })
        {
            return new ApiResponse<List<UserData>>
            {
                IsSuccess = true,
                Message = "Users found",
                Data = userResponse.Data
            };
        }

        return new ApiResponse<List<UserData>>
        {
            IsSuccess = false,
            Message = userResponse?.Message ?? "No users found",
            Data = new List<UserData>()
        };
    }
}