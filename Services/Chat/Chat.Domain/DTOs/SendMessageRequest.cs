namespace Chat.Domain.DTOs;

public class SendMessageRequest
{
    public string ConversationId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}