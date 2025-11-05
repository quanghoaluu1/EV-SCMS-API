namespace Chat.Domain.DTOs;

public class TransferToStaffRequest
{
    public string ConversationId { get; set; } = string.Empty;
    public string? StaffId { get; set; }
    public string Reason { get; set; } = string.Empty;
}