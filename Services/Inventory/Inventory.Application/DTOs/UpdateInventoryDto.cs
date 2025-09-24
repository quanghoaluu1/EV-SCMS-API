using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public class UpdateInventoryDto
    {
        public string Name { get; set; } = string.Empty;
        public InventoryCategoryEnum Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = "Available";
    }

}
