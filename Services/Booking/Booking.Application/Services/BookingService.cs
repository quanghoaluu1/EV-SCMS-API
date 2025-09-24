using Booking.Application.DTOs;
using Booking.Application.IServices;
using Booking.Infrastructure.IRepository;
using Mapster;

namespace Booking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingDTOs> CreateAsync(BookingCreateDto dto)
        {
            var booking = dto.Adapt<Domain.Entities.Booking>();
            var created = await _bookingRepository.AddAsync(booking);
            return created.Adapt<BookingDTOs>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _bookingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BookingDTOs>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Adapt<IEnumerable<BookingDTOs>>();
        }

        public async Task<IEnumerable<BookingDTOs>> GetByCustomerIdAsync(Guid customerId)
        {
            var bookings = await _bookingRepository.GetByCustomerIdAsync(customerId);
            return bookings.Adapt<IEnumerable<BookingDTOs>>();
        }

        public async Task<BookingDTOs?> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking?.Adapt<BookingDTOs>();
        }

        public async Task<BookingDTOs?> UpdateAsync(Guid id, BookingUpdateDto dto)
        {
            var existing = await _bookingRepository.GetByIdAsync(id);
            if (existing == null) return null;

            dto.Adapt(existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _bookingRepository.UpdateAsync(existing);
            return updated?.Adapt<BookingDTOs>();
        }
    }
}
