namespace Chat.Application.Interfaces;

public interface IPresenceService
{
    Task SetUserOnlineAsync(string userId, string connectionId);
    Task SetUserOfflineAsync(string userId);
    Task<bool> IsUserOnlineAsync(string userId);
    Task<List<string>> GetOnlineUsersAsync();
    Task<string?> GetConnectionIdAsync(string userId);
    Task SetTypingAsync(string conversationId, string userId, bool isTyping);
    Task<List<string>> GetTypingUsersAsync(string conversationId);
}