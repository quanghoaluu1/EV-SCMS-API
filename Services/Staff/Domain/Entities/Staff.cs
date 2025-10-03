using Domain.Enum;

namespace Domain.Entities;

public class Staff
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public StaffPosition Position { get; set; }
    public DateOnly HireDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();
    public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public ICollection<PerformanceLog> PerformanceLogs { get; set; } = new List<PerformanceLog>();
}