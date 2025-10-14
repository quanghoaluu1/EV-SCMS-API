using CustomerGrpc;

namespace Auth.Infrastructure.Interfaces;

public interface ICustomerGrpcGateway
{
    Task<CreateCustomerReply> CreateCustomer(Guid userId, string address);
}