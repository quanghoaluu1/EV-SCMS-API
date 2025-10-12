using BookingGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IServices
{
    public interface IBookingGrpcGateway
    {
        Task<GetBookingByIdReply> GetBookingById(Guid bookingId);
    }
}
