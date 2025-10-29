using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IServices
{
    public interface IBookingGrpcGateway
    {
        Task<BookingResponse> GetBookingById(Guid bookingId);
    }

    public class BookingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
