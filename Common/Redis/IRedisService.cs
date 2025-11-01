namespace Common.Redis;

public interface IRedisService
{
    Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> GetStringAsync(string key);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> SetHashAsync(string key, Dictionary<string, string> hashFields);
    Task<Dictionary<string, string>> GetHashAsync(string key);
    Task<bool> SetHashFieldAsync(string key, string field, string value);
    Task<string?> GetHashFieldAsync(string key, string field);
    Task<bool> AddToSetAsync(string key, string value);
    Task<bool> RemoveFromSetAsync(string key, string value);
    Task<List<string>> GetSetMembersAsync(string key);
    Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
    Task<TimeSpan?> GetTtlAsync(string key);
}