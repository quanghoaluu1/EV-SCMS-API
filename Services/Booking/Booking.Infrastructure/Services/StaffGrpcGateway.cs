using Booking.Infrastructure.IServices;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using StaffGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Services
{
    public class StaffGrpcGateway : IStaffGrpcGateway
    {
        private readonly Staff.StaffClient _client;
        private readonly ILogger<StaffGrpcGateway> _logger;

        public StaffGrpcGateway(Staff.StaffClient client, ILogger<StaffGrpcGateway> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetStaffByIdReply> GetStaffById(Guid staffId)
        {
            try
            {
                _logger.LogInformation("Calling Staff service to validate staff {StaffId}", staffId);

                var request = new GetStaffByIdRequest
                {
                    StaffId = staffId.ToString()
                };

                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetStaffByIdAsync(request, callOptions);

                _logger.LogInformation("Staff service responded: Success={Success}, Message={Message}",
                    response.Success, response.Message);

                return response;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Staff service is unavailable");
                return new GetStaffByIdReply
                {
                    Success = false,
                    Message = "Staff service is currently unavailable. Please ensure the Staff service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Staff service");
                return new GetStaffByIdReply
                {
                    Success = false,
                    Message = $"Staff service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Staff service");
                return new GetStaffByIdReply
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

    }
}
