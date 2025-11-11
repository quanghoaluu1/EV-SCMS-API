using Chat.Domain.Entities;
using Chat.Domain.Enums;
using Chat.Infrastructure.Interfaces;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _conversations;

    public ConversationRepository(IMongoDbContext context)
    {
        _conversations = context.GetCollection<Conversation>("conversations");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var customerIndex = Builders<Conversation>.IndexKeys.Ascending(x => x.CustomerId);
        _conversations.Indexes.CreateOne(new CreateIndexModel<Conversation>(customerIndex));

        var staffIndex = Builders<Conversation>.IndexKeys.Ascending(x => x.AssignedStaffId);
        _conversations.Indexes.CreateOne(new CreateIndexModel<Conversation>(staffIndex));
    }

    public async Task<Conversation> CreateConversationAsync(Conversation conversation)
    {
        await _conversations.InsertOneAsync(conversation);
        return conversation;
    }

    public async Task<Conversation?> GetConversationByIdAsync(string conversationId)
    {
        return await _conversations.Find(x => x.Id == conversationId).FirstOrDefaultAsync();
    }

    public async Task<List<Conversation>> GetConversationsByCustomerIdAsync(string customerId)
    {
        return await _conversations
            .Find(x => x.CustomerId == customerId)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync();
    }

    public async Task<List<Conversation>> GetConversationsByStaffIdAsync(string staffId)
    {
        return await _conversations
            .Find(x => x.AssignedStaffId == staffId)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync();
    }

    public Task<List<Conversation>> GetAllConversationAsync()
    {
        return _conversations
            .Find(_ => true)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateConversationAsync(Conversation conversation)
    {
        conversation.UpdatedAt = DateTime.UtcNow;
        var result = await _conversations.ReplaceOneAsync(x => x.Id == conversation.Id, conversation);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AssignStaffAsync(string conversationId, string staffId, string staffName)
    {
        var update = Builders<Conversation>.Update
            .Set(x => x.AssignedStaffId, staffId)
            .Set(x => x.AssignedStaffName, staffName)
            .Set(x => x.Type, ConversationType.Staff)  // ✅ Đổi từ HumanSupport sang Staff
            .Set(x => x.Status, ConversationStatus.Active) // ✅ Đổi trạng thái thành Active khi có staff nhận
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _conversations.UpdateOneAsync(x => x.Id == conversationId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateStatusAsync(string conversationId, ConversationStatus status)
    {
        var update = Builders<Conversation>.Update
            .Set(x => x.Status, status)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _conversations.UpdateOneAsync(x => x.Id == conversationId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateLastMessageAsync(string conversationId, string lastMessage)
    {
        var update = Builders<Conversation>.Update
            .Set(x => x.LastMessage, lastMessage)
            .Set(x => x.LastMessageAt, DateTime.UtcNow)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _conversations.UpdateOneAsync(x => x.Id == conversationId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> IncrementUnreadCountAsync(string conversationId)
    {
        var update = Builders<Conversation>.Update
            .Inc(x => x.UnreadCount, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _conversations.UpdateOneAsync(x => x.Id == conversationId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> ResetUnreadCountAsync(string conversationId)
    {
        var update = Builders<Conversation>.Update
            .Set(x => x.UnreadCount, 0)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _conversations.UpdateOneAsync(x => x.Id == conversationId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<List<Conversation>> GetWaitingConversationsAsync()
    {
        return await _conversations
            .Find(x => x.Status == ConversationStatus.Waiting)
            .SortBy(x => x.CreatedAt) // Oldest first
            .ToListAsync();
    }
}