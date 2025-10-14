using Auth.Infrastructure.Interfaces;
using CustomerGrpc;

namespace Auth.Infrastructure.GrpcClients;

public class CustomerGrpcGateway : ICustomerGrpcGateway
{
    private readonly CustomerGrpc.CustomerService.CustomerServiceClient _client;
    public CustomerGrpcGateway(CustomerGrpc.CustomerService.CustomerServiceClient client)
    {
        _client = client;
    }

    public async Task<CreateCustomerReply> CreateCustomer(Guid userId, string address)
    {
        var request = new CreateCustomerRequest
        {
            UserId = userId.ToString(),
            Address = address
        };
        return await _client.CreateCustomerAsync(request);
    }
}