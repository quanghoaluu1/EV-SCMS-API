namespace Chat.Domain.DTOs;

public class AssignStaffRequest
{
    public string ConversationId { get; set; } = string.Empty;
    public string StaffId { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    
}