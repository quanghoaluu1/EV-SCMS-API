using Chat.Domain.Entities;

namespace Chat.Application.Interfaces;

public interface IAiChatService
{
    Task<string> GenerateResponseAsync(string conversationId, string userMessage, List<ChatMessage> conversationHistory);
    Task<bool> ShouldTransferToHumanAsync(string userMessage);
    Task<string> GetIntentAsync(string userMessage);
}