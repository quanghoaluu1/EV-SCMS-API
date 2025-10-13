using Domain.Enum;

namespace Application.DTOs;

public class UpdateStaffShiftDto
{
    public Guid? StaffId { get; set; }
    public Guid? ShiftId { get; set; }
    public DateOnly? WorkDate { get; set; }
    public StaffShiftStatus? Status { get; set; }
}

