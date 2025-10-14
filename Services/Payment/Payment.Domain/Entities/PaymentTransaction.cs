using Payment.Domain.Enum;

namespace Payment.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public string? OrderCode { get; set; }
    public string? DataJson { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? CustomerId { get; set; }
    public double Amount { get; set; }
    public PaymentMethod? Method { get; set; }
    public PaymentStatus? Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}