using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs
{
    public class BookingCreateDto
    {
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid? StaffId { get; set; }

        public BookingEnums ServiceType { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string? Notes { get; set; }
        public decimal? TotalCost { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
    }
}