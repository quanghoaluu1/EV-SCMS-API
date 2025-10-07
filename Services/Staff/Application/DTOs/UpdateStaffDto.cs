using Domain.Enum;

namespace Application.DTOs;

public class UpdateStaffDto
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
}