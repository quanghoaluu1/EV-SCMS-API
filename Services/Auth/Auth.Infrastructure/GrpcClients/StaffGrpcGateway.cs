using Auth.Infrastructure.Interfaces;
using StaffGrpc;

namespace Auth.Infrastructure.GrpcClients;

public class StaffGrpcGateway : IStaffGrpcGateway
{
    private readonly Staff.StaffClient _client;
    public StaffGrpcGateway(Staff.StaffClient client)
    {
        _client = client;
    }
    public async Task<CreateStaffReply> CreateStaffAsync(Guid userId, DateOnly hireDate, CancellationToken ct = default)
    {
        var req = new CreateStaffRequest
        {
            UserId = userId.ToString(),
            HireDate = hireDate.ToString("yyyy-MM-dd")
        };
        return await _client.CreateStaffAsync(req, cancellationToken: ct);
    }
}