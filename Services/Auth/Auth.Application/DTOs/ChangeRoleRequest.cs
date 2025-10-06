using Auth.Domain.Enums;

namespace Auth.Application.DTOs;

public class ChangeRoleRequest
{
    public Guid UserId { get; set; }
    public RoleEnum Role { get; set; }
}