using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Domain.Entities;

public class Conservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    [BsonElement("customerId")]
    public string CustomerId { get; set; } = string.Empty;
    [BsonElement("assignedStaffId")]
    public string? AssignedStaffId { get; set; } = null;
    [BsonElement("assignedStaffName")]
    public string? AssignedStaffName { get; set; } = null;
}