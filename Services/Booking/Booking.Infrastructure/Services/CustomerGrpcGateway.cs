using Booking.Infrastructure.IServices;
using CustomerGrpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Services
{
    public class CustomerGrpcGateway : ICustomerGrpcGateway
    {
        private readonly CustomerService.CustomerServiceClient _client;
        private readonly ILogger<CustomerGrpcGateway> _logger;

        public CustomerGrpcGateway(CustomerService.CustomerServiceClient client, ILogger<CustomerGrpcGateway> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetCustomerByIdReply> GetCustomerById(Guid customerId)
        {
            try
            {
                _logger.LogInformation("Calling Customer service to validate customer {CustomerId}", customerId);

                var request = new GetCustomerByIdRequest
                {
                    CustomerId = customerId.ToString()
                };

                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetCustomerByIdAsync(request, callOptions);

                _logger.LogInformation("Customer service responded: Success={Success}, Message={Message}",
                    response.Success, response.Message);

                return response;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Customer service is unavailable");
                return new GetCustomerByIdReply
                {
                    Success = false,
                    Message = "Customer service is currently unavailable. Please ensure the Customer service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Customer service");
                return new GetCustomerByIdReply
                {
                    Success = false,
                    Message = $"Customer service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Customer service");
                return new GetCustomerByIdReply
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

    }
}
