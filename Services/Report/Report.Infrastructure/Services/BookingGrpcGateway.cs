using BookingGrpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Report.Application.IServices;

namespace Report.Infrastructure.Services
{
    public class BookingGrpcGateway : IBookingGrpcGateway
    {
        private readonly BookingGrpcService.BookingGrpcServiceClient _client;
        private readonly ILogger<BookingGrpcGateway> _logger;

        public BookingGrpcGateway(
            BookingGrpcService.BookingGrpcServiceClient client,
            ILogger<BookingGrpcGateway> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<BookingStatsResult> GetBookingStatsAsync()
        {
            try
            {
                _logger.LogInformation("Calling Booking service to get booking statistics");

                var request = new GetBookingStatsRequest();
                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetBookingStatsAsync(request, callOptions);

                _logger.LogInformation("Booking service responded: Success={Success}, Total={Total}",
                    response.Success, response.Total);

                return new BookingStatsResult
                {
                    Success = response.Success,
                    Message = response.Message,
                    Total = response.Total,
                    Pending = response.Pending,
                    InProgress = response.InProgress,
                    Completed = response.Completed,
                    Cancelled = response.Cancelled,
                    Today = response.Today,
                    ThisWeek = response.ThisWeek,
                    ThisMonth = response.ThisMonth,
                    CompletionRate = response.CompletionRate
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Booking service is unavailable");
                return new BookingStatsResult
                {
                    Success = false,
                    Message = "Booking service is currently unavailable. Please ensure the Booking service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Booking service");
                return new BookingStatsResult
                {
                    Success = false,
                    Message = $"Booking service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Booking service");
                return new BookingStatsResult
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        public async Task<RevenueResult> GetRevenueAsync()
        {
            try
            {
                _logger.LogInformation("Calling Booking service to get revenue statistics");

                var request = new GetRevenueRequest();
                var deadline = DateTime.UtcNow.AddSeconds(30);
                var callOptions = new CallOptions(deadline: deadline);

                var response = await _client.GetRevenueAsync(request, callOptions);

                _logger.LogInformation("Booking service responded: Success={Success}, Total={Total}",
                    response.Success, response.Total);

                return new RevenueResult
                {
                    Success = response.Success,
                    Message = response.Message,
                    Total = response.Total,
                    ThisMonth = response.ThisMonth,
                    ThisWeek = response.ThisWeek,
                    Today = response.Today,
                    Paid = response.Paid,
                    Unpaid = response.Unpaid,
                    AveragePerBooking = response.AveragePerBooking
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError(ex, "Booking service is unavailable");
                return new RevenueResult
                {
                    Success = false,
                    Message = "Booking service is currently unavailable. Please ensure the Booking service is running."
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when calling Booking service");
                return new RevenueResult
                {
                    Success = false,
                    Message = $"Booking service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Booking service");
                return new RevenueResult
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}