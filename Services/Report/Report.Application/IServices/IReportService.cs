using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Report.Application.DTOs.ReportDTOs;

namespace Report.Application.IServices
{
    public interface IReportService
    {
        Task<DashboardResponseDto> GetDashboardAsync(DashboardFilterDto? filter = null);
        Task<RevenueDto> GetRevenueStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<BookingStatsDto> GetBookingStatsAsync();
        Task<InventoryAlertDto> GetInventoryAlertsAsync();
        Task<RevenueChartDto> GetRevenueChartDataAsync(int months = 6);
    }
}
