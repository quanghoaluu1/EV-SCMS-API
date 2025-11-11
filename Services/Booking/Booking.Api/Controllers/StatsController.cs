using Microsoft.AspNetCore.Mvc;
using Booking.Application.IServices;
using Booking.Domain.Enums;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<StatsController> _logger;

        public StatsController(IBookingService bookingService, ILogger<StatsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Get booking statistics for reporting
        /// </summary>
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookingStats()
        {
            try
            {
                _logger.LogInformation("Getting booking statistics for reporting");

                // Get all bookings (increased page size to get more complete stats)
                var allBookings = await _bookingService.GetAllAsync(1, 10000);
                var bookingsList = allBookings.ToList();

                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                var stats = new BookingStatsResult
                {
                    Success = true,
                    Message = "Booking statistics retrieved successfully",
                    Total = bookingsList.Count,
                    Pending = bookingsList.Count(b => b.Status == BookingStatusEnum.Pending),
                    InProgress = bookingsList.Count(b => b.Status == BookingStatusEnum.Processing), 
                    Completed = bookingsList.Count(b => b.Status == BookingStatusEnum.Completed),
                    Cancelled = bookingsList.Count(b => b.Status == BookingStatusEnum.Cancelled),
                    Today = bookingsList.Count(b => b.ScheduledDate >= today && b.ScheduledDate < today.AddDays(1)),
                    ThisWeek = bookingsList.Count(b => b.ScheduledDate >= startOfWeek && b.ScheduledDate < startOfWeek.AddDays(7)),
                    ThisMonth = bookingsList.Count(b => b.ScheduledDate >= startOfMonth && b.ScheduledDate < startOfMonth.AddMonths(1)),
                    CompletionRate = bookingsList.Count > 0 ? 
                        (double)bookingsList.Count(b => b.Status == BookingStatusEnum.Completed) / bookingsList.Count * 100 : 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking statistics");
                return Ok(new BookingStatsResult
                {
                    Success = false,
                    Message = $"Error retrieving booking statistics: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get revenue statistics for reporting
        /// </summary>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            try
            {
                _logger.LogInformation("Getting revenue statistics for reporting");

                // For now, we'll return placeholder revenue data since BookingListDto doesn't have TotalCost
                // In a real implementation, you would need to either:
                // 1. Add TotalCost and PaymentStatus to BookingListDto
                // 2. Create a separate method in BookingService that returns full booking details
                // 3. Query the database directly for revenue calculations

                var revenue = new RevenueResult
                {
                    Success = true,
                    Message = "Revenue statistics retrieved successfully (placeholder data)",
                    Total = 0, // Placeholder - would calculate from actual booking costs
                    ThisMonth = 0,
                    ThisWeek = 0,
                    Today = 0,
                    Paid = 0,
                    Unpaid = 0,
                    AveragePerBooking = 0
                };

                _logger.LogInformation("Revenue statistics retrieved (placeholder implementation)");
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue statistics");
                return Ok(new RevenueResult
                {
                    Success = false,
                    Message = $"Error retrieving revenue statistics: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Health check for stats service
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                Success = true,
                Message = "Booking stats service is running",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // Result DTOs for the stats endpoints
    public class BookingStatsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Today { get; set; }
        public int ThisWeek { get; set; }
        public int ThisMonth { get; set; }
        public double CompletionRate { get; set; }
    }

    public class RevenueResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double Total { get; set; }
        public double ThisMonth { get; set; }
        public double ThisWeek { get; set; }
        public double Today { get; set; }
        public double Paid { get; set; }
        public double Unpaid { get; set; }
        public double AveragePerBooking { get; set; }
    }
}