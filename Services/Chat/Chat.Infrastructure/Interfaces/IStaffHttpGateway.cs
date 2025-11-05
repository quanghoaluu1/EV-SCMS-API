using System.Text.Json.Serialization;

namespace Chat.Infrastructure.Interfaces;

public interface IStaffHttpGateway
{
    Task<ApiResponse<UserData>> GetStaffById(Guid userId);
    Task<ApiResponse<UserData>> GetStaffByUserId(Guid userId);
    Task<ApiResponse<List<UserData>>> GetStaffByRoleAsync(int role);
    Task<ApiResponse<List<UserData>>> GetServiceAdvisorsAsync();

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
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    [JsonPropertyName("staffId")]
    public string StaffId { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    [JsonPropertyName("role")]
    public int Role { get; set; }
}