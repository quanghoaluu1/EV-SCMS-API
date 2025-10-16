using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Application.DTOs
{
    public class ReportDTOs
    {
        public class DashboardResponseDto
        {
            public RevenueDto Revenue { get; set; } = new();
            public InventoryAlertDto Inventory { get; set; } = new();
            public BookingStatsDto Bookings { get; set; } = new();
            public MaintenanceStatsDto Maintenances { get; set; } = new();
            public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        }

        public class RevenueDto
        {
            public decimal Total { get; set; }

            
            public decimal ThisMonth { get; set; }


            public decimal ThisWeek { get; set; }

            public decimal Today { get; set; }


            public decimal Paid { get; set; }
            public decimal Unpaid { get; set; }


            public decimal AveragePerBooking { get; set; }
        }

        public class InventoryAlertDto
        {
            public int LowStockCount { get; set; }
            public int OutOfStockCount { get; set; }
            public List<InventoryItemDto> LowStockItems { get; set; } = new();
            public List<InventoryItemDto> OutOfStockItems { get; set; } = new();
            public string AlertLevel { get; set; } = "Good"; // Good, Warning, Critical
        }

        public class InventoryItemDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public int MinimumStockLevel { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        public class BookingStatsDto
        {
            public int Total { get; set; }
            public int Pending { get; set; }
            public int Confirmed { get; set; }
            public int InProgress { get; set; }
            public int Completed { get; set; }
            public int Cancelled { get; set; }


            public int Today { get; set; }
            public int ThisWeek { get; set; }
            public int ThisMonth { get; set; }


            public decimal CompletionRate { get; set; }
        }

        public class MaintenanceStatsDto
        {
            public int Total { get; set; }
            public int Pending { get; set; }
            public int InProgress { get; set; }
            public int Completed { get; set; }
            public int Cancelled { get; set; }
        }

        public class RevenueChartDto
        {
            public List<ChartDataPoint> MonthlyRevenue { get; set; } = new();
            public List<ChartDataPoint> WeeklyRevenue { get; set; } = new();
        }

        public class ChartDataPoint
        {
            public string Label { get; set; } = string.Empty; // "Jan 2025", "Week 1"
            public decimal Value { get; set; }
            public DateTime Date { get; set; }
        }

        public class DashboardFilterDto
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IncludeChartData { get; set; } = false;
        }
    }
}
