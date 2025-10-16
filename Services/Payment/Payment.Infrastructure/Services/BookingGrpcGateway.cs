
using BookingGrpc;
using Payment.Infrastructure.Interfaces;

namespace Payment.Infrastructure.Services;

public class BookingGrpcGateway : IBookingGrpcGateway
{
    private readonly BookingGrpcService.BookingGrpcServiceClient _client;
    
    public BookingGrpcGateway(BookingGrpcService.BookingGrpcServiceClient client)
    {
        _client = client;
    }
    
    public async Task<GetBookingByIdReply> GetBookingById(Guid bookingId)
    {
        var request = new GetBookingByIdRequest
        {
            BookingId = bookingId.ToString()
        };
        
        return await _client.GetBookingByIdAsync(request);
    }
}