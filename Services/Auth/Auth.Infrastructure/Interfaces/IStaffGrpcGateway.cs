using StaffGrpc;

namespace Auth.Infrastructure.Interfaces;

public interface IStaffGrpcGateway
{
    Task<CreateStaffReply> CreateStaffAsync(Guid userId, DateOnly hireDate, CancellationToken ct = default);
}