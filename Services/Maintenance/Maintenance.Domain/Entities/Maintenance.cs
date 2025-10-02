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
        public DateTime ServiceDate { get; set; }
        public int Odometer { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MaintenanceEnums Status { get; set; }= MaintenanceEnums.Pending;
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
