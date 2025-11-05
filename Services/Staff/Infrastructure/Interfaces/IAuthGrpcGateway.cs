namespace Infrastructure.Interfaces;

public interface IAuthGrpcGateway
{ 
    Task<ApiResponse<UserData>> GetUserById(Guid userId);
    Task<ApiResponse<List<UserData>>> GetUserByRole(int role);
}

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public class UserData
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Role { get; set; }
    public DateTime CreatedAt { get; set; }
}