using Maintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities
{
    public class Maintenance
    {
        public Guid MaintenanceId { get; set; }
        public Guid VehicleId { get; set; }    
        public Guid CustomerId { get; set; }
        public Guid BookingId { get; set; }
        public Guid StaffId { get; set; } // technicianId
        public DateTime ServiceDate { get; set; }
        public int Odometer { get; set; }
        public MaintenanceServiceTypeEnum ServiceType { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public MaintenanceEnums Status { get; set; }= MaintenanceEnums.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
