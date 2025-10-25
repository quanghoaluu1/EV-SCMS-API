namespace Payment.Domain.Dtos;

public class PayOsWebhookPayload
{
    public bool Success { get; set; }
    public PayOSWebhookData Data { get; set; }
}

public class PayOSWebhookData
{
    public long OrderCode { get; set; }
    public string PaymentLinkId { get; set; }
    public long Amount { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }   // "00" nghĩa là thanh toán thành công
    public string Desc { get; set; }
    public DateTime TransactionDateTime { get; set; }
}