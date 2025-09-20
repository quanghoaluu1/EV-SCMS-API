namespace Auth.Application.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
    
    public string Message { get; set; } = string.Empty;
}