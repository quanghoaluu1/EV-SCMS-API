using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.DTOs
{
    public class BookingUpdateDto
    {
        public Guid? StaffId { get; set; }
        public BookingEnums ServiceType { get; set; }
        public DateTime ScheduledDate { get; set; }
        public BookingStatusEnum Status { get; set; }
        public string? Notes { get; set; }
        public decimal? TotalCost { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
    }
}
