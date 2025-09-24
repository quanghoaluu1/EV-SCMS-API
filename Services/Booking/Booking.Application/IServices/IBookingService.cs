using Booking.Application.DTOs;

namespace Booking.Application.IServices
{
    public interface IBookingService
    {
        Task<BookingDTOs> CreateAsync(BookingCreateDto dto);
        Task<BookingDTOs?> GetByIdAsync(Guid id);
        Task<IEnumerable<BookingDTOs>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<BookingDTOs>> GetAllAsync();
        Task<BookingDTOs?> UpdateAsync(Guid id, BookingUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}