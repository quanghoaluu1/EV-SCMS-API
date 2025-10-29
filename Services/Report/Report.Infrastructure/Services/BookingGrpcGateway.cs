using Microsoft.Extensions.Logging;
using Report.Application.IServices;
using System.Net.Http;
using System.Net.Http.Json;

namespace Report.Infrastructure.Services
{
    public class BookingGrpcGateway : IBookingGrpcGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookingGrpcGateway> _logger;

        public BookingGrpcGateway(
            HttpClient httpClient,
            ILogger<BookingGrpcGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BookingStatsResult> GetBookingStatsAsync()
        {
            try
            {
                _logger.LogInformation("Calling Booking service to get booking statistics");

                var response = await _httpClient.GetAsync("api/stats/bookings");

                if (response.IsSuccessStatusCode)
                {
                    var stats = await response.Content.ReadFromJsonAsync<BookingStatsResult>();
                    
                    _logger.LogInformation("Booking service responded: Success={Success}, Total={Total}",
                        stats?.Success ?? false, stats?.Total ?? 0);

                    return stats ?? new BookingStatsResult
                    {
                        Success = false,
                        Message = "Failed to parse response"
                    };
                }
                else
                {
                    _logger.LogError("Booking service returned error status: {StatusCode}", response.StatusCode);
                    return new BookingStatsResult
                    {
                        Success = false,
                        Message = $"Booking service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Booking service");
                return new BookingStatsResult
                {
                    Success = false,
                    Message = "Booking service is currently unavailable. Please ensure the Booking service is running."
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

                var response = await _httpClient.GetAsync("api/stats/revenue");

                if (response.IsSuccessStatusCode)
                {
                    var revenue = await response.Content.ReadFromJsonAsync<RevenueResult>();
                    
                    _logger.LogInformation("Booking service responded: Success={Success}, Total={Total}",
                        revenue?.Success ?? false, revenue?.Total ?? 0);

                    return revenue ?? new RevenueResult
                    {
                        Success = false,
                        Message = "Failed to parse response"
                    };
                }
                else
                {
                    _logger.LogError("Booking service returned error status: {StatusCode}", response.StatusCode);
                    return new RevenueResult
                    {
                        Success = false,
                        Message = $"Booking service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Booking service");
                return new RevenueResult
                {
                    Success = false,
                    Message = "Booking service is currently unavailable. Please ensure the Booking service is running."
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