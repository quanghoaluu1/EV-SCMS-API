using Domain.Enum;

namespace Application.DTOs;

public class UpdateLeaveRequestDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveRequestStatus? Status { get; set; }
}

