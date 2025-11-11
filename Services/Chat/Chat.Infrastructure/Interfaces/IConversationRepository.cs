using Chat.Domain.Entities;
using Chat.Domain.Enums;

namespace Chat.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<Conversation> CreateConversationAsync(Conversation conversation);
    Task<Conversation?> GetConversationByIdAsync(string conversationId);
    Task<List<Conversation>> GetConversationsByCustomerIdAsync(string customerId);
    Task<List<Conversation>> GetConversationsByStaffIdAsync(string staffId);
    Task<List<Conversation>> GetAllConversationAsync();
    Task<bool> UpdateConversationAsync(Conversation conversation);
    Task<bool> AssignStaffAsync(string conversationId, string staffId, string staffName);
    Task<bool> UpdateStatusAsync(string conversationId, ConversationStatus status);
    Task<bool> UpdateLastMessageAsync(string conversationId, string lastMessage);
    Task<bool> IncrementUnreadCountAsync(string conversationId);
    Task<bool> ResetUnreadCountAsync(string conversationId);
    Task<List<Conversation>> GetWaitingConversationsAsync();
}