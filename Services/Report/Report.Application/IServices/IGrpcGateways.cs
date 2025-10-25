namespace Report.Application.IServices
{
    public interface IBookingGrpcGateway
    {
        Task<BookingStatsResult> GetBookingStatsAsync();
        Task<RevenueResult> GetRevenueAsync();
    }

    public interface IMaintenanceGrpcGateway
    {
        Task<MaintenanceStatsResult> GetMaintenanceStatsAsync();
    }

    // Result DTOs to decouple from GRPC types
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