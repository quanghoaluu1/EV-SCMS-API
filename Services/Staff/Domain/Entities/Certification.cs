namespace Domain.Entities;

public class Certification
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiredDate { get; set; }
    
    public Staff Staff { get; set; } = null!;
}