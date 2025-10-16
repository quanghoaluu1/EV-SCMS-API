using Microsoft.AspNetCore.Mvc;
using Report.Application.IServices;
using static Report.Application.DTOs.ReportDTOs;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IBookingGrpcGateway _bookingGrpcGateway;
        private readonly IMaintenanceGrpcGateway _maintenanceGrpcGateway;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            IReportService reportService,
            IBookingGrpcGateway bookingGrpcGateway,
            IMaintenanceGrpcGateway maintenanceGrpcGateway,
            ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _bookingGrpcGateway = bookingGrpcGateway;
            _maintenanceGrpcGateway = maintenanceGrpcGateway;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard([FromQuery] DashboardFilterDto? filter = null)
        {
            try
            {
                var dashboard = await _reportService.GetDashboardAsync(filter);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard");
                return StatusCode(500, new { error = "Failed to generate dashboard" });
            }
        }

        [HttpGet("revenue")]
        [ProducesResponseType(typeof(RevenueDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRevenue(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var revenue = await _reportService.GetRevenueStatsAsync(startDate, endDate);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating revenue");
                return StatusCode(500, new { error = "Failed to calculate revenue" });
            }
        }

        [HttpGet("bookings")]
        [ProducesResponseType(typeof(BookingStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookingStats()
        {
            try
            {
                var stats = await _reportService.GetBookingStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating booking stats");
                return StatusCode(500, new { error = "Failed to calculate booking stats" });
            }
        }

        [HttpGet("inventory/alerts")]
        [ProducesResponseType(typeof(InventoryAlertDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInventoryAlerts()
        {
            try
            {
                var alerts = await _reportService.GetInventoryAlertsAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory alerts");
                return StatusCode(500, new { error = "Failed to get inventory alerts" });
            }
        }

        [HttpGet("revenue/chart")]
        [ProducesResponseType(typeof(RevenueChartDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRevenueChart([FromQuery] int months = 6)
        {
            if (months < 1 || months > 24)
                return BadRequest(new { error = "Months must be between 1 and 24" });

            try
            {
                var chartData = await _reportService.GetRevenueChartDataAsync(months);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating revenue chart");
                return StatusCode(500, new { error = "Failed to generate chart data" });
            }
        }

        // NEW: Direct gRPC endpoints for real-time data
        [HttpGet("bookings/stats/realtime")]
        [ProducesResponseType(typeof(BookingStatsResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRealtimeBookingStats()
        {
            try
            {
                _logger.LogInformation("Getting real-time booking statistics via gRPC");

                var stats = await _bookingGrpcGateway.GetBookingStatsAsync();

                if (!stats.Success)
                {
                    _logger.LogWarning("Failed to get booking stats: {Message}", stats.Message);
                    return StatusCode(503, new { error = stats.Message });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Real-time booking statistics retrieved successfully",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time booking statistics");
                return StatusCode(500, new { error = "Failed to get real-time booking statistics" });
            }
        }

        [HttpGet("revenue/realtime")]
        [ProducesResponseType(typeof(RevenueResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRealtimeRevenue()
        {
            try
            {
                _logger.LogInformation("Getting real-time revenue data via gRPC");

                var revenue = await _bookingGrpcGateway.GetRevenueAsync();

                if (!revenue.Success)
                {
                    _logger.LogWarning("Failed to get revenue data: {Message}", revenue.Message);
                    return StatusCode(503, new { error = revenue.Message });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Real-time revenue data retrieved successfully",
                    Data = revenue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time revenue data");
                return StatusCode(500, new { error = "Failed to get real-time revenue data" });
            }
        }

        [HttpGet("maintenance/stats/realtime")]
        [ProducesResponseType(typeof(MaintenanceStatsResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRealtimeMaintenanceStats()
        {
            try
            {
                _logger.LogInformation("Getting real-time maintenance statistics via gRPC");

                var stats = await _maintenanceGrpcGateway.GetMaintenanceStatsAsync();

                if (!stats.Success)
                {
                    _logger.LogWarning("Failed to get maintenance stats: {Message}", stats.Message);
                    return StatusCode(503, new { error = stats.Message });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Real-time maintenance statistics retrieved successfully",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time maintenance statistics");
                return StatusCode(500, new { error = "Failed to get real-time maintenance statistics" });
            }
        }

        [HttpGet("dashboard/realtime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRealtimeDashboard()
        {
            try
            {
                _logger.LogInformation("Getting real-time dashboard data via gRPC");

                // Call all gRPC services concurrently for better performance
                var bookingStatsTask = _bookingGrpcGateway.GetBookingStatsAsync();
                var revenueTask = _bookingGrpcGateway.GetRevenueAsync();
                var maintenanceStatsTask = _maintenanceGrpcGateway.GetMaintenanceStatsAsync();

                await Task.WhenAll(bookingStatsTask, revenueTask, maintenanceStatsTask);

                var bookingStats = await bookingStatsTask;
                var revenue = await revenueTask;
                var maintenanceStats = await maintenanceStatsTask;

                var dashboard = new
                {
                    Success = bookingStats.Success && revenue.Success && maintenanceStats.Success,
                    Message = "Real-time dashboard data retrieved",
                    Data = new
                    {
                        BookingStats = bookingStats,
                        Revenue = revenue,
                        MaintenanceStats = maintenanceStats,
                        GeneratedAt = DateTime.UtcNow
                    },
                    ServiceStatus = new
                    {
                        BookingService = bookingStats.Success ? "Available" : "Unavailable",
                        MaintenanceService = maintenanceStats.Success ? "Available" : "Unavailable"
                    }
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time dashboard data");
                return StatusCode(500, new { error = "Failed to get real-time dashboard data" });
            }
        }
    }
}
