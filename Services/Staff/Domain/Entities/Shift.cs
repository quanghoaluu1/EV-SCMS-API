namespace Domain.Entities;

public class Shift
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
    public ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();
}