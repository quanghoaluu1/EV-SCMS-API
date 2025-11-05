using Chat.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Domain.Entities;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    [BsonElement("customerId")]
    public string CustomerId { get; set; } = string.Empty;
    [BsonElement("customerName")]
    public string CustomerName { get; set; } = string.Empty;
    [BsonElement("assignedStaffId")]
    public string? AssignedStaffId { get; set; } = null;
    [BsonElement("assignedStaffName")]
    public string? AssignedStaffName { get; set; } = null;
    [BsonElement("status")]
    public ConversationStatus Status { get; set; }
    [BsonElement("type")]
    public ConversationType Type { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastMessageAt")]
    public DateTime? LastMessageAt { get; set; }

    [BsonElement("lastMessage")]
    public string? LastMessage { get; set; }

    [BsonElement("unreadCount")]
    public int UnreadCount { get; set; } = 0;

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonIgnore]
    public bool IsWaitingForStaff => Status == ConversationStatus.Waiting;

    [BsonIgnore]
    public bool HasAssignedStaff => !string.IsNullOrEmpty(AssignedStaffId);

    [BsonIgnore]
    public bool IsActive => Status == ConversationStatus.Active;
}