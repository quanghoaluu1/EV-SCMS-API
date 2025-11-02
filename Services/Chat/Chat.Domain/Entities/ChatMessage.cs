using Chat.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Domain.Entities;

public class ChatMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    [BsonElement("conversationId")]
    public string ConversationId { get; set; } = string.Empty;
    [BsonElement("senderId")]
    public string SenderId { get; set; } = string.Empty;
    [BsonElement("senderName")]
    public string SenderName { get; set; } = string.Empty;
    [BsonElement("senderType")]
    public SenderType SenderType { get; set; }
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;
    [BsonElement("readAt")]
    public DateTime? ReadAt { get; set; } = null;
}