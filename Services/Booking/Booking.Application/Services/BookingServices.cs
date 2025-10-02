using Booking.Application.IServices;
using Booking.Domain.Enums;
using Booking.Infrastructure.IRepository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Application.Services
{
    public class BookingServices : IBookingService
    {
        private readonly IBookingRepository _repository;

        public BookingServices(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        {
            var booking = dto.Adapt<Domain.Entities.Booking>();
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(booking);
            return created.Adapt<BookingResponseDto>();
        }

        public async Task<BookingResponseDto?> UpdateAsync(Guid bookingId, BookingUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(bookingId);
            if (existing == null) return null;

            dto.Adapt(existing); // update in-place
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return updated?.Adapt<BookingResponseDto>();
        }

        public async Task<bool> DeleteAsync(Guid bookingId)
        {
            return await _repository.DeleteAsync(bookingId);
        }

        public async Task<BookingResponseDto?> GetByIdAsync(Guid bookingId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);
            return booking?.Adapt<BookingResponseDto>();
        }

        public async Task<IEnumerable<BookingListDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var bookings = await _repository.GetAllAsync(pageNumber, pageSize);
            return bookings.Adapt<IEnumerable<BookingListDto>>();
        }

        public async Task<IEnumerable<BookingListDto>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10)
        {
            var bookings = await _repository.GetByCustomerIdAsync(customerId, pageNumber, pageSize);
            return bookings.Adapt<IEnumerable<BookingListDto>>();
        }

        public async Task<bool> UpdateStatusAsync(Guid bookingId, BookingStatusEnum status)
        {
            return await _repository.UpdateStatusAsync(bookingId, status);
        }

        public async Task<bool> AssignStaffAsync(Guid bookingId, Guid staffId)
        {
            return await _repository.AssignStaffAsync(bookingId, staffId);
        }
    }
}
