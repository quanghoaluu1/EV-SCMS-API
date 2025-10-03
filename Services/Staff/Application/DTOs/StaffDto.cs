namespace Application.DTOs;

public class StaffDto
{
    public Guid StaffId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string Status { get; set; } = null!;
}