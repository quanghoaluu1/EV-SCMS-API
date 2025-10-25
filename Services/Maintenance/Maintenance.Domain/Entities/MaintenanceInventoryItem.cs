using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities
{
    public class MaintenanceInventoryItem
    {
        public Guid Id { get; set; }
        public Guid MaintenanceId { get; set; }
        public Guid InventoryId { get; set; }
        public int Quantity { get; set; } // How many of this item were used
        public decimal UnitCost { get; set; } // Cost per unit at time of use
        public decimal TotalCost { get; set; } // Quantity * UnitCost
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Maintenance Maintenance { get; set; } = null!;
    }
}
