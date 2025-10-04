namespace Domain.Entities;

public class PerformanceLog
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public Staff Staff { get; set; } = null!;
    public Guid WorkOrderId { get; set; } 
    public int CompletedTasks { get; set; }
    public decimal AvgTimePerTask { get; set; }
    public decimal Rating { get; set; }
    public DateTime LogDate { get; set; }
}