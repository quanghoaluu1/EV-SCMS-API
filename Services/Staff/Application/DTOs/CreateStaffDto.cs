using Domain.Enum;

namespace Application.DTOs;

public class CreateStaffDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public StaffPosition Position { get; set; }
}