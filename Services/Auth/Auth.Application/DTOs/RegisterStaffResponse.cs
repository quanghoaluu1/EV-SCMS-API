namespace Auth.Application.DTOs;

public class RegisterStaffResponse
{
    public AuthResponse Auth { get; set; } = new AuthResponse();
    public Guid StaffId { get; set; }
}