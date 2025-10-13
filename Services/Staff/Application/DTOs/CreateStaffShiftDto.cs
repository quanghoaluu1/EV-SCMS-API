using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateStaffShiftDto
{
    [Required]
    public Guid StaffId { get; set; }
    [Required]
    public Guid ShiftId { get; set; }
    [Required]
    public DateOnly WorkDate { get; set; }
}

