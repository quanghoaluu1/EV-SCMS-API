using Auth.Domain.Enums;

namespace Auth.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = null!;
    public string Status { get; set; } = "Active";
    public RoleEnum Role { get; set; } = RoleEnum.Customer;
    public DateTime CreatedAt { get; set; }
}