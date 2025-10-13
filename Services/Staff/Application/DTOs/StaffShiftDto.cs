using Domain.Enum;

namespace Application.DTOs;

public class StaffShiftDto
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public Guid ShiftId { get; set; }
    public DateOnly WorkDate { get; set; }
    public StaffShiftStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ShiftDto? Shift { get; set; }
    public StaffDto? Staff { get; set; }
}

