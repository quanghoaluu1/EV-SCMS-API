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
        public Guid BookingId { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; } 
        public Guid? StaffId { get; set; } //Staf assigned to the booking / first available staff if null
        public BookingEnums ServiceType { get; set; }
        public DateTime ScheduledDate { get; set; }
        public BookingStatusEnum Status { get; set; } = BookingStatusEnum.Pending;
        public string? Notes { get; set; }
        public decimal? TotalCost { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
