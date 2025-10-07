using Domain.Enum;

namespace Application.DTOs;

public class CreateStaffDto
{
    public Guid UserId { get; set; }
    public DateOnly HireDate { get; set; }
}