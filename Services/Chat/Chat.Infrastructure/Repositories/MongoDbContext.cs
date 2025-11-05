using Chat.Domain.Settings;
using Chat.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        
        var mongoUrl = new MongoUrl(settings.Value.ConnectionString);

        var mongoSettings = MongoClientSettings.FromUrl(mongoUrl);
    
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        
        var client = new MongoClient(mongoSettings);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}