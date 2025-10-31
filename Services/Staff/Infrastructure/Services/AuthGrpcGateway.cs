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
            var response = await _httpClient.GetAsync($"api/Auth/id?userId={userId}");
            var userResponse = await response.Content.ReadFromJsonAsync<GetUserByIdResponse>();
            Console.WriteLine("userResponse:" + JsonSerializer.Serialize(userResponse));
            
            if (userResponse is { IsSuccess: true, Data: not null })
            {
                return new GetUserByIdResponse
                {
                    IsSuccess = true,
                    Message = "User found",
                    Data = userResponse.Data
                };
            }
            else
            {
                return new GetUserByIdResponse
                {
                    IsSuccess = false,
                    Message = $"User not found: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new GetUserByIdResponse
            {
                IsSuccess = false,
                Message = $"Error getting user: {ex.Message}"
            };
        }
    }
}