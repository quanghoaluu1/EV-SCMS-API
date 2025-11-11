using Microsoft.AspNetCore.Mvc;
using Maintenance.Application.IServices;
using Maintenance.Domain.Enums;

namespace Maintenance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly ILogger<StatsController> _logger;

        public StatsController(IMaintenanceService maintenanceService, ILogger<StatsController> logger)
        {
            _maintenanceService = maintenanceService;
            _logger = logger;
        }

        /// <summary>
        /// Get maintenance statistics for reporting
        /// </summary>
        [HttpGet("maintenance")]
        public async Task<IActionResult> GetMaintenanceStats()
        {
            try
            {
                _logger.LogInformation("Getting maintenance statistics for reporting");

                // Get all maintenance records (increased page size to get more complete stats)
                var allMaintenance = await _maintenanceService.GetAllAsync(1, 10000);
                var maintenanceList = allMaintenance.ToList();

                var stats = new MaintenanceStatsResult
                {
                    Success = true,
                    Message = "Maintenance statistics retrieved successfully",
                    Total = maintenanceList.Count,
                    Pending = maintenanceList.Count(m => m.Status == MaintenanceEnums.Pending),
                    InProgress = maintenanceList.Count(m => m.Status == MaintenanceEnums.InProgress),
                    Completed = maintenanceList.Count(m => m.Status == MaintenanceEnums.Completed),
                    Cancelled = maintenanceList.Count(m => m.Status == MaintenanceEnums.Cancelled)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance statistics");
                return Ok(new MaintenanceStatsResult
                {
                    Success = false,
                    Message = $"Error retrieving maintenance statistics: {ex.Message}"
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
                Message = "Maintenance stats service is running",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // Result DTO for the maintenance stats endpoint
    public class MaintenanceStatsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }
}