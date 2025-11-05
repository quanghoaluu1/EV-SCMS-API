using Chat.Domain.DTOs;
using Chat.Domain.Entities;
using Chat.Domain.Enums;

namespace Chat.Application.Interfaces;

public interface IChatService
{
    Task<Conversation> CreateConversationAsync(CreateConversationRequest request);
    Task<ChatMessage> SendMessageAsync(string userId, string userName, SenderType senderType, SendMessageRequest request);
    Task<List<ChatMessage>> GetConversationMessagesAsync(string conversationId, int skip = 0, int limit = 50);
    Task<List<Conversation>> GetUserConversationsAsync(string userId, SenderType userType);
    Task<bool> TransferToStaffAsync(TransferToStaffRequest request);
    Task<bool> MarkMessagesAsReadAsync(string conversationId, string userId);
    Task<Conversation?> GetConversationAsync(string conversationId);
    Task<List<Conversation>> GetAllConversationsAsync();
    Task<bool> AssignStaffToConversationAsync(AssignStaffRequest request);
    Task<List<Conversation>> GetMyAssignedConversationsAsync(string staffId);
    Task<List<Conversation>> GetWaitingConversationsAsync();
    Task<List<Conversation>> GetStaffConversationsAsync(string staffId);
    Task<bool> RequestTransferToStaffAsync(string conversationId);
}