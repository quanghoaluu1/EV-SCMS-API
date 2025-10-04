using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Application.IServices
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(BookingCreateDto dto);
        Task<BookingResponseDto?> UpdateAsync(Guid bookingId, BookingUpdateDto dto);
        Task<bool> DeleteAsync(Guid bookingId);
        Task<BookingResponseDto?> GetByIdAsync(Guid bookingId);
        Task<IEnumerable<BookingListDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<BookingListDto>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10);
        Task<bool> UpdateStatusAsync(Guid bookingId, BookingStatusEnum status);
        Task<bool> AssignStaffAsync(Guid bookingId, Guid staffId);
    }
}
