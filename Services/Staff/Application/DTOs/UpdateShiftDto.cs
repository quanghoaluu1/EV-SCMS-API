using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class UpdateShiftDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

