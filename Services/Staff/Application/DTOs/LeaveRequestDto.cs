using Domain.Enum;

namespace Application.DTOs;

public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; } = null!;
    public LeaveRequestStatus Status { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public StaffDto? Staff { get; set; }
}

