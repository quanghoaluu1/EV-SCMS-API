using Chat.Application.Interfaces;
using Common.Redis;

namespace Chat.Application.Services;

public class PresenceService : IPresenceService
{
    private readonly IRedisService _redis;
    private const string OnlineUsersKey = "online_users";
    private const string UserConnectionPrefix = "user_connection:";
    private const string TypingPrefix = "typing:";
    private readonly TimeSpan _onlineExpiry = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _typingExpiry = TimeSpan.FromSeconds(5);

    public PresenceService(IRedisService redis)
    {
        _redis = redis;
    }

    public async Task SetUserOnlineAsync(string userId, string connectionId)
    {
        await _redis.AddToSetAsync(OnlineUsersKey, userId);
        await _redis.SetStringAsync($"{UserConnectionPrefix}{userId}", connectionId, _onlineExpiry);
    }

    public async Task SetUserOfflineAsync(string userId)
    {
        await _redis.RemoveFromSetAsync(OnlineUsersKey, userId);
        await _redis.DeleteAsync($"{UserConnectionPrefix}{userId}");
    }

    public async Task<bool> IsUserOnlineAsync(string userId)
    {
        return await _redis.ExistsAsync($"{UserConnectionPrefix}{userId}");
    }

    public async Task<List<string>> GetOnlineUsersAsync()
    {
        return await _redis.GetSetMembersAsync(OnlineUsersKey);
    }

    public async Task<string?> GetConnectionIdAsync(string userId)
    {
        return await _redis.GetStringAsync($"{UserConnectionPrefix}{userId}");
    }

    public async Task SetTypingAsync(string conversationId, string userId, bool isTyping)
    {
        var key = $"{TypingPrefix}{conversationId}";
        
        if (isTyping)
        {
            await _redis.AddToSetAsync(key, userId);
            await _redis.SetExpiryAsync(key, _typingExpiry);
        }
        else
        {
            await _redis.RemoveFromSetAsync(key, userId);
        }
    }

    public async Task<List<string>> GetTypingUsersAsync(string conversationId)
    {
        var key = $"{TypingPrefix}{conversationId}";
        return await _redis.GetSetMembersAsync(key);
    }
}