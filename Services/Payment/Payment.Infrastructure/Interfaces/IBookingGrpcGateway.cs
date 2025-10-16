using BookingGrpc;

namespace Payment.Infrastructure.Interfaces;

public interface IBookingGrpcGateway
{
    public Task<GetBookingByIdReply> GetBookingById(Guid bookingId);
}