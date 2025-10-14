using CustomerGrpc;

namespace Payment.Infrastructure.Interfaces;

public interface ICustomerGrpcGateway
{
    Task<GetCustomerByIdReply> GetCustomerById(Guid customerId);
}