namespace Payment.Domain.Enum;

public enum PaymentStatus
{
    Unpaid = 0,       // Chưa thanh toán
    Paid = 1,      // Đã thanh toán
    Cancelled = 2 // Đã hủy thanh toán
}