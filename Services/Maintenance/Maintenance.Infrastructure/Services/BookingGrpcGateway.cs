using BookingGrpc;
using Grpc.Core;
using Maintenance.Infrastructure.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Services
{
    public class BookingGrpcGateway : IBookingGrpcGateway
    {
        private readonly BookingGrpcService.BookingGrpcServiceClient _client;
        private readonly ILogger<BookingGrpcGateway> _logger;

        public BookingGrpcGateway(BookingGrpcService.BookingGrpcServiceClient client, ILogger<BookingGrpcGateway> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GetBookingByIdReply> GetBookingById(Guid bookingId)
        {
            try
            {
                _logger.LogInformation("Calling Booking service to validate booking {BookingId}", bookingId);

                var request = new GetBookingByIdRequest
                {
                    BookingId = bookingId.ToString()
                };

                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetBookingByIdAsync(request, callOptions);

                _logger.LogInformation("Booking service responded: Success={Success}, Message={Message}",
                    response.Success, response.Message);

                return response;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Booking service is unavailable");
                return new GetBookingByIdReply
                {
                    Success = false,
                    Message = "Booking service is currently unavailable. Please ensure the Booking service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Booking service");
                return new GetBookingByIdReply
                {
                    Success = false,
                    Message = $"Booking service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Booking service");
                return new GetBookingByIdReply
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
