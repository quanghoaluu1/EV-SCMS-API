namespace Auth.Infrastructure.Interfaces;

public interface ICustomerGrpcGateway
{
    Task<CreateCustomerResponse> CreateCustomer(Guid userId, string address);
}

public class CreateCustomerResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
}