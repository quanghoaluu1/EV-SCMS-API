namespace Infrastructure.Interfaces;

public interface IAuthGrpcGateway
{ 
    Task<GetUserByIdResponse> GetUserById(Guid userId);
}

public class GetUserByIdResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}