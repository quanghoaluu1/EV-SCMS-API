using Domain.Enum;

namespace Domain.Entities;

public class StaffShift
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public Guid ShiftId { get; set; }
    public DateOnly WordDate { get; set; }
    public StaffShiftStatus Status { get; set; } = StaffShiftStatus.Assigned;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Staff Staff { get; set; } = null!;
    public Shift Shift { get; set; } = null!;
}