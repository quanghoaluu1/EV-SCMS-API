using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public InventoryCategoryEnum Category { get; set; }
        public int Quantity { get; set; }
        public int MinimumStockLevel { get; set; } = 5;
        public decimal UnitPrice { get; set; }
        public InventoryStatusEnum Status { get; set; } = InventoryStatusEnum.Available;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
