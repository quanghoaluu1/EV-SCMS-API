using Chat.Domain.Enums;

namespace Chat.Domain.DTOs;

public class CreateConversationRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public ConversationType Type { get; set; } = ConversationType.AiBot;
    public string? InitialMessage { get; set; }
}