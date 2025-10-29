using System.Text.Json.Serialization;

namespace Customer.Infrastructure.IRepository;

public interface IAuthGrpcGateway
{
    Task<ApiResponse<UserData>> GetUserById(Guid userId);
}

public class ApiResponse<T>
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("data")]
    public T Data { get; set; }
}
public class UserData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    [JsonPropertyName("role")]
    public int Role { get; set; }
}
