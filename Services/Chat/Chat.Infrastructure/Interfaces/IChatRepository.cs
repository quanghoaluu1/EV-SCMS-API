using Chat.Domain.Entities;

namespace Chat.Infrastructure.Interfaces;

public interface IChatRepository
{
    Task<ChatMessage> CreateMessageAsync(ChatMessage message);
    Task<ChatMessage?> GetMessageByIdAsync(string messageId);
    Task<List<ChatMessage>> GetMessagesByConversationIdAsync(string conversationId, int skip = 0, int limit = 50);
    Task<bool> MarkAsReadAsync(string messageId);
    Task<int> GetUnreadCountAsync(string conversationId, string userId);
    Task<bool> DeleteMessageAsync(string messageId);
}