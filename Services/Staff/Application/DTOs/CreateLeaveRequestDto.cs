using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateLeaveRequestDto
{
    [Required]
    public Guid StaffId { get; set; }
    [Required]
    public DateOnly StartDate { get; set; }
    [Required]
    public DateOnly EndDate { get; set; }
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = null!;
}

