namespace Auth.Infrastructure.Interfaces;

public interface IStaffGrpcGateway
{
    Task<CreateStaffResponse> CreateStaffAsync(Guid userId, DateOnly hireDate, CancellationToken ct = default);
}

public class CreateStaffResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string StaffId { get; set; } = string.Empty;
}