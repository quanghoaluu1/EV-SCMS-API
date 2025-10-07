using Auth.Domain.Enums;

namespace Auth.Application.DTOs;

public class RegisterStaffRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public RoleEnum Role { get; set; }
    public DateOnly HireDate { get; set; }
}