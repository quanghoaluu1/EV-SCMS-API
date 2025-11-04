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
                        Status = GetStatusAsString(data)
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

        public async Task<BookingUpdateResponse> UpdateBookingStatusAsync(Guid bookingId, string newStatus)
        {
            try
            {
                _logger.LogInformation("Updating booking status for {BookingId} to {NewStatus}", bookingId, newStatus);

                // The booking API expects the status as a JSON value in the request body
                var requestBody = JsonSerializer.Serialize(newStatus);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"api/Booking/{bookingId}/status", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Booking status updated successfully for {BookingId}", bookingId);

                    return new BookingUpdateResponse
                    {
                        Success = true,
                        Message = "Booking status updated successfully",
                        BookingId = bookingId.ToString(),
                        NewStatus = newStatus
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to update booking status. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);

                    return new BookingUpdateResponse
                    {
                        Success = false,
                        Message = $"Failed to update booking status: {response.StatusCode}",
                        BookingId = bookingId.ToString(),
                        NewStatus = newStatus
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when updating booking status for {BookingId}", bookingId);
                return new BookingUpdateResponse
                {
                    Success = false,
                    Message = "Booking service is currently unavailable for status update",
                    BookingId = bookingId.ToString(),
                    NewStatus = newStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when updating booking status for {BookingId}", bookingId);
                return new BookingUpdateResponse
                {
                    Success = false,
                    Message = $"Unexpected error updating booking status: {ex.Message}",
                    BookingId = bookingId.ToString(),
                    NewStatus = newStatus
                };
            }
        }

        private string GetStatusAsString(JsonElement data)
        {
            if (data.TryGetProperty("status", out var status))
            {
                if (status.ValueKind == JsonValueKind.Number)
                {
                    // Handle enum as number (0=Pending, 1=Processing, 2=Completed, 3=Cancelled)
                    var statusValue = status.GetInt32();
                    return statusValue switch
                    {
                        0 => "Pending",
                        1 => "Processing", 
                        2 => "Completed",
                        3 => "Cancelled",
                        _ => "Unknown"
                    };
                }
                else if (status.ValueKind == JsonValueKind.String)
                {
                    // Handle enum as string (new format after JsonStringEnumConverter)
                    return status.GetString() ?? string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
