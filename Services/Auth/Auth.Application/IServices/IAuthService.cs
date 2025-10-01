using Auth.Application.DTOs;
using Auth.Domain.Entities;

namespace Auth.Application.IServices;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    string GenerateToken(User user);
    Task<UserDto?> GetUserByEmailAsync(string email);
}