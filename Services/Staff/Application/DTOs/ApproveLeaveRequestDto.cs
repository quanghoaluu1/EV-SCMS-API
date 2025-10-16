namespace Application.DTOs;

public class ApproveLeaveRequestDto
{
    public bool IsApproved { get; set; }
    public Guid ApprovedBy { get; set; }
    public string? ApprovalNotes { get; set; }
}

