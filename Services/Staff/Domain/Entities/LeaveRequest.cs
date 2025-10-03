using Domain.Enum;

namespace Domain.Entities;

public class LeaveRequest
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = null!;
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
    
    public Staff Staff { get; set; } = null!;

}