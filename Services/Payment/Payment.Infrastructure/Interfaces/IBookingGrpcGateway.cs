namespace Payment.Infrastructure.Interfaces;

public interface IBookingGrpcGateway
{
    public Task<BookingResponse> GetBookingById(Guid bookingId);
}

public class BookingResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string StaffId { get; set; } = string.Empty;
    public string VehicleId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public int TotalCost { get; set; }
    public int ServiceType { get; set; }

}