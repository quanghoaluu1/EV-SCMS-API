using Chat.Domain.Enums;

namespace Chat.Domain.DTOs;

public class CreateConversationRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public SenderType Type { get; set; } = SenderType.AiBot;
    public string? InitialMessage { get; set; }
}