using Common.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Common.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _database;
    private readonly string _instanceName;

    public RedisService(IOptions<RedisSettings> settings)
    {
        var redis = ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
        _database = redis.GetDatabase();
        _instanceName = settings.Value.InstanceName;
    }

    private string GetKey(string key) => $"{_instanceName}{key}";

    public async Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        return await _database.StringSetAsync(GetKey(key), value, expiry);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        var value = await _database.StringGetAsync(GetKey(key));
        return value.HasValue ? value.ToString() : null;
    }
    public async Task<bool> DeleteAsync(string key)
    {
        return await _database.KeyDeleteAsync(GetKey(key));
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(GetKey(key));
    }

    public async Task<bool> SetHashAsync(string key, Dictionary<string, string> hashFields)
    {
        var entries = hashFields.Select(kv => new HashEntry(kv.Key, kv.Value)).ToArray();
        await _database.HashSetAsync(GetKey(key), entries);
        return true;
    }

    public async Task<Dictionary<string, string>> GetHashAsync(string key)
    {
        var entries = await _database.HashGetAllAsync(GetKey(key));
        return entries.ToDictionary(
            entry => entry.Name.ToString(),
            entry => entry.Value.ToString()
        );
    }

    public async Task<bool> SetHashFieldAsync(string key, string field, string value)
    {
        return await _database.HashSetAsync(GetKey(key), field, value);
    }

    public async Task<string?> GetHashFieldAsync(string key, string field)
    {
        var value = await _database.HashGetAsync(GetKey(key), field);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> AddToSetAsync(string key, string value)
    {
        return await _database.SetAddAsync(GetKey(key), value);
    }

    public async Task<bool> RemoveFromSetAsync(string key, string value)
    {
        return await _database.SetRemoveAsync(GetKey(key), value);
    }

    public async Task<List<string>> GetSetMembersAsync(string key)
    {
        var members = await _database.SetMembersAsync(GetKey(key));
        return members.Select(m => m.ToString()).ToList();
    }

    public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
    {
        return await _database.KeyExpireAsync(GetKey(key), expiry);
    }
    public async Task<int> StringIncrementAsync(string key, int value)
    {
        return (int)await _database.StringIncrementAsync(GetKey(key), value);
    }

}