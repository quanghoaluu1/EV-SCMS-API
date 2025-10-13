using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public static class InventoryDTOs
    {
        public class CreateInventoryDto
        {
            [Required(ErrorMessage = "Name is required")]
            [MaxLength(200)]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Category is required")]
            public InventoryCategoryEnum Category { get; set; }

            [Required]
            [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
            public int Quantity { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Minimum stock level must be at least 1")]
            public int MinimumStockLevel { get; set; } = 5;

            [Required]
            [Range(0, double.MaxValue, ErrorMessage = "Unit price must be non-negative")]
            public decimal UnitPrice { get; set; }
        }

        public class UpdateInventoryDto
        {
            [MaxLength(200)]
            public string? Name { get; set; }

            public InventoryCategoryEnum? Category { get; set; }

            [Range(0, int.MaxValue)]
            public int? Quantity { get; set; }

            [Range(1, int.MaxValue)]
            public int? MinimumStockLevel { get; set; }

            [Range(0, double.MaxValue)]
            public decimal? UnitPrice { get; set; }

            public InventoryStatusEnum? Status { get; set; }
        }

        public class InventoryResponseDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public InventoryCategoryEnum Category { get; set; }
            public string CategoryName { get; set; } = string.Empty; 
            public int Quantity { get; set; }
            public int MinimumStockLevel { get; set; }
            public decimal UnitPrice { get; set; }
            public InventoryStatusEnum Status { get; set; }
            public string StatusName { get; set; } = string.Empty; 
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public bool IsLowStock => Quantity <= MinimumStockLevel && Quantity > 0;
            public bool IsOutOfStock => Quantity <= 0;
        }

        public class InventoryListDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public InventoryCategoryEnum Category { get; set; }
            public int Quantity { get; set; }
            public int MinimumStockLevel { get; set; }
            public InventoryStatusEnum Status { get; set; }
            public bool IsLowStock => Quantity <= MinimumStockLevel && Quantity > 0;
        }


        // For stock adjustment
        public class AdjustStockDto
        {
            [Required]
            [Range(-int.MaxValue, int.MaxValue)]
            public int QuantityChange { get; set; } // Positive = increase, Negative = decrease

            public string? Notes { get; set; }
        }

        // For alert response
        public class InventoryAlertDto
        {
            public int LowStockCount { get; set; }
            public int OutOfStockCount { get; set; }
            public IEnumerable<InventoryListDto> LowStockItems { get; set; } = new List<InventoryListDto>();
            public IEnumerable<InventoryListDto> OutOfStockItems { get; set; } = new List<InventoryListDto>();
        }

        // For dashboard
        public class InventorySummaryDto
        {
            public int TotalItems { get; set; }
            public int TotalQuantity { get; set; }
            public decimal TotalValue { get; set; }
            public int LowStockCount { get; set; }
            public int OutOfStockCount { get; set; }
            public string HealthStatus { get; set; } = "Good"; // Good, Warning, Critical
        }
    }
}