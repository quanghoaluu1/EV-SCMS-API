using MongoDB.Driver;

namespace Chat.Infrastructure.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}