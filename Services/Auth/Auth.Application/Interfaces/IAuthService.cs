using Auth.Application.DTOs;
using Auth.Application.DTOs.Auth;
using Common.DTOs;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);

    Task<Result<RegisterStaffResponse>> RegisterStaffAsync(RegisterStaffRequest request,
        CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<bool>> ChangeRoleAsync(ChangeRoleRequest request);
    Task<Result<UserDto>> GetUserByIdAsync(Guid id);
}