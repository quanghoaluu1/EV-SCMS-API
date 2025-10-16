using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid? StaffId { get; set; }
        public BookingEnums ServiceType { get; set; }
        public DateTime ScheduledDate { get; set; }
        public BookingStatusEnum Status { get; set; } = BookingStatusEnum.Pending;
        public string? Notes { get; set; }
        public int? TotalCost { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
