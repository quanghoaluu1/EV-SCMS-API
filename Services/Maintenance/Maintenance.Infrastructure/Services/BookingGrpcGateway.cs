using Maintenance.Infrastructure.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Services
{
    public class BookingGrpcGateway : IBookingGrpcGateway
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookingGrpcGateway> _logger;

        public BookingGrpcGateway(HttpClient httpClient, ILogger<BookingGrpcGateway> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BookingResponse> GetBookingById(Guid bookingId)
        {
            try
            {
                _logger.LogInformation("Calling Booking service to validate booking {BookingId}", bookingId);

                var response = await _httpClient.GetAsync($"api/Booking/{bookingId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    var root = jsonDoc.RootElement;
                    
                    var data = root.TryGetProperty("data", out var dataElement) ? dataElement : root;
                    
                    _logger.LogInformation("Booking service responded successfully");

                    return new BookingResponse
                    {
                        Success = true,
                        Message = "Booking found",
                        BookingId = bookingId.ToString(),
                        CustomerId = data.TryGetProperty("customerId", out var custId) ? custId.ToString() : string.Empty,
                        VehicleId = data.TryGetProperty("vehicleId", out var vehId) ? vehId.ToString() : string.Empty,
                        Status = data.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Booking not found: {BookingId}", bookingId);
                    return new BookingResponse
                    {
                        Success = false,
                        Message = "Booking not found"
                    };
                }
                else
                {
                    _logger.LogError("Booking service returned error status: {StatusCode}", response.StatusCode);
                    return new BookingResponse
                    {
                        Success = false,
                        Message = $"Booking service error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when calling Booking service");
                return new BookingResponse
                {
                    Success = false,
                    Message = "Booking service is currently unavailable. Please ensure the Booking service is running."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Booking service");
                return new BookingResponse
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
