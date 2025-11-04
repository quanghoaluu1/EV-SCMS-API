using System.Net.Http.Json;
using System.Text.Json;
using Chat.Infrastructure.Interfaces;

namespace Chat.Infrastructure.Services;

public class StaffHttpGateway : IStaffHttpGateway
{
    private readonly HttpClient _httpClient;
    public StaffHttpGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ApiResponse<UserData>> GetStaffById(Guid staffId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Staff/{staffId}");

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
    
    public async Task<ApiResponse<UserData>> GetStaffByUserId(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Staff/userId/{userId}");

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
    public async Task<ApiResponse<List<UserData>>> GetStaffByRoleAsync(int role)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/users/role/{role}");
            
            var userResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserData>>>();

            return new ApiResponse<List<UserData>>()
            {
                IsSuccess = userResponse.IsSuccess,
                Message = userResponse.Message,
                Data = userResponse.Data
            };

        }
        catch (Exception ex)
        {
            return new ApiResponse<List<UserData>>();
        }
    }

    public async Task<ApiResponse<List<UserData>>> GetServiceAdvisorsAsync()
    {
        return await GetStaffByRoleAsync(3);
    }
}