using Payment.Infrastructure.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Payment.Infrastructure.Services;

public class BookingGrpcGateway : IBookingGrpcGateway
{
    private readonly HttpClient _httpClient;
    
    public BookingGrpcGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<BookingResponse> GetBookingById(Guid bookingId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Booking/{bookingId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;
                
                var data = root.TryGetProperty("data", out var dataElement) ? dataElement : root;

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
            else
            {
                return new BookingResponse
                {
                    Success = false,
                    Message = $"Booking not found: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new BookingResponse
            {
                Success = false,
                Message = $"Error getting booking: {ex.Message}"
            };
        }
    }
}