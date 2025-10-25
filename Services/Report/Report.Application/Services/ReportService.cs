using Microsoft.Extensions.Logging;
using Report.Application.IServices;
using static Report.Application.DTOs.ReportDTOs;

namespace Report.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IBookingGrpcGateway _bookingGrpcGateway;
        private readonly IMaintenanceGrpcGateway _maintenanceGrpcGateway;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IBookingGrpcGateway bookingGrpcGateway,
            IMaintenanceGrpcGateway maintenanceGrpcGateway,
            ILogger<ReportService> logger)
        {
            _bookingGrpcGateway = bookingGrpcGateway;
            _maintenanceGrpcGateway = maintenanceGrpcGateway;
            _logger = logger;
        }

        public async Task<DashboardResponseDto> GetDashboardAsync(DashboardFilterDto? filter = null)
        {
            try
            {
                _logger.LogInformation("Generating dashboard report");

                // Call all services in parallel for better performance
                var bookingStatsTask = GetBookingStatsAsync();
                var revenueTask = GetRevenueStatsAsync();
                var maintenanceStatsTask = GetMaintenanceStatsAsync();
                var inventoryAlertsTask = GetInventoryAlertsAsync();

                await Task.WhenAll(bookingStatsTask, revenueTask, maintenanceStatsTask, inventoryAlertsTask);

                var dashboard = new DashboardResponseDto
                {
                    Bookings = await bookingStatsTask,
                    Revenue = await revenueTask,
                    Maintenances = await maintenanceStatsTask,
                    Inventory = await inventoryAlertsTask,
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Dashboard generated successfully");
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard");
                throw;
            }
        }

        public async Task<RevenueDto> GetRevenueStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Fetching revenue statistics from Booking service");

                var revenueResponse = await _bookingGrpcGateway.GetRevenueAsync();

                if (!revenueResponse.Success)
                {
                    _logger.LogWarning("Failed to get revenue from Booking service: {Message}", revenueResponse.Message);
                    throw new Exception($"Revenue calculation failed: {revenueResponse.Message}");
                }

                return new RevenueDto
                {
                    Total = (decimal)revenueResponse.Total,
                    ThisMonth = (decimal)revenueResponse.ThisMonth,
                    ThisWeek = (decimal)revenueResponse.ThisWeek,
                    Today = (decimal)revenueResponse.Today,
                    Paid = (decimal)revenueResponse.Paid,
                    Unpaid = (decimal)revenueResponse.Unpaid,
                    AveragePerBooking = (decimal)revenueResponse.AveragePerBooking
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue statistics");
                throw;
            }
        }

        public async Task<BookingStatsDto> GetBookingStatsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching booking statistics from Booking service");

                var bookingResponse = await _bookingGrpcGateway.GetBookingStatsAsync();

                if (!bookingResponse.Success)
                {
                    _logger.LogWarning("Failed to get booking stats from Booking service: {Message}", bookingResponse.Message);
                    throw new Exception($"Booking statistics failed: {bookingResponse.Message}");
                }

                return new BookingStatsDto
                {
                    Total = bookingResponse.Total,
                    Pending = bookingResponse.Pending,
                    Confirmed = 0, // Note: Booking service returns InProgress instead of Confirmed
                    InProgress = bookingResponse.InProgress,
                    Completed = bookingResponse.Completed,
                    Cancelled = bookingResponse.Cancelled,
                    Today = bookingResponse.Today,
                    ThisWeek = bookingResponse.ThisWeek,
                    ThisMonth = bookingResponse.ThisMonth,
                    CompletionRate = (decimal)bookingResponse.CompletionRate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking statistics");
                throw;
            }
        }

        public async Task<MaintenanceStatsDto> GetMaintenanceStatsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching maintenance statistics from Maintenance service");

                var maintenanceResponse = await _maintenanceGrpcGateway.GetMaintenanceStatsAsync();

                if (!maintenanceResponse.Success)
                {
                    _logger.LogWarning("Failed to get maintenance stats from Maintenance service: {Message}", maintenanceResponse.Message);
                    throw new Exception($"Maintenance statistics failed: {maintenanceResponse.Message}");
                }

                return new MaintenanceStatsDto
                {
                    Total = maintenanceResponse.Total,
                    Pending = maintenanceResponse.Pending,
                    InProgress = maintenanceResponse.InProgress,
                    Completed = maintenanceResponse.Completed,
                    Cancelled = maintenanceResponse.Cancelled
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance statistics");
                throw;
            }
        }

        public async Task<InventoryAlertDto> GetInventoryAlertsAsync()
        {
            try
            {
                _logger.LogInformation("Generating inventory alerts (placeholder implementation)");

                // TODO: Implement inventory alerts when Inventory service GRPC is available
                // For now, return empty alerts
                return new InventoryAlertDto
                {
                    LowStockCount = 0,
                    OutOfStockCount = 0,
                    AlertLevel = "Good",
                    LowStockItems = new List<InventoryItemDto>(),
                    OutOfStockItems = new List<InventoryItemDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory alerts");
                throw;
            }
        }

        public async Task<RevenueChartDto> GetRevenueChartDataAsync(int months = 6)
        {
            try
            {
                _logger.LogInformation("Generating revenue chart data for {Months} months", months);

                // TODO: This would require enhanced GRPC methods to get historical data
                // For now, return placeholder data
                var chartData = new RevenueChartDto();

                var currentDate = DateTime.UtcNow;
                for (int i = months - 1; i >= 0; i--)
                {
                    var date = currentDate.AddMonths(-i);
                    chartData.MonthlyRevenue.Add(new ChartDataPoint
                    {
                        Label = date.ToString("MMM yyyy"),
                        Date = date,
                        Value = 0 // Would need historical data from services
                    });
                }

                _logger.LogInformation("Revenue chart data generated");
                return chartData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating revenue chart data");
                throw;
            }
        }
    }
}
