using Chat.Domain.Entities;
using Chat.Infrastructure.Interfaces;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<ChatMessage> _messages;
    public ChatRepository(IMongoDbContext context)
    {
        _messages = context.GetCollection<ChatMessage>("messages");
        CreateIndexes();
    }
    private void CreateIndexes()
    {
        var indexKeys = Builders<ChatMessage>.IndexKeys
            .Ascending(x => x.ConversationId)
            .Descending(x => x.Timestamp);
        
        var indexOptions = new CreateIndexOptions { Background = true };
        var indexModel = new CreateIndexModel<ChatMessage>(indexKeys, indexOptions);
        
        _messages.Indexes.CreateOne(indexModel);
    }
    
    public async Task<ChatMessage> CreateMessageAsync(ChatMessage message)
    {
        await _messages.InsertOneAsync(message);
        return message;
    }

    public async Task<ChatMessage?> GetMessageByIdAsync(string messageId)
    {
        return await _messages.Find(x => x.Id == messageId).FirstOrDefaultAsync();
    }

    public async Task<List<ChatMessage>> GetMessagesByConversationIdAsync(string conversationId, int skip = 0, int limit = 50)
    {
        return await _messages
            .Find(x => x.ConversationId == conversationId)
            .SortByDescending(x => x.Timestamp)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<bool> MarkAsReadAsync(string messageId)
    {
        var update = Builders<ChatMessage>.Update
            .Set(x => x.IsRead, true)
            .Set(x => x.ReadAt, DateTime.UtcNow);

        var result = await _messages.UpdateOneAsync(x => x.Id == messageId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<int> GetUnreadCountAsync(string conversationId, string userId)
    {
        var count = await _messages.CountDocumentsAsync(x => 
            x.ConversationId == conversationId && 
            !x.IsRead && 
            x.SenderId != userId);
        
        return (int)count;
    }

    public async Task<bool> DeleteMessageAsync(string messageId)
    {
        var result = await _messages.DeleteOneAsync(x => x.Id == messageId);
        return result.DeletedCount > 0;
    }
}