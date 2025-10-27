using Booking.Application.IServices;
using Booking.Domain.Enums;
using Booking.Infrastructure.IRepository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Infrastructure.IServices;
using Messaging;
using Microsoft.Extensions.Configuration;
using static Booking.Application.DTOs.BookingDTOs;

namespace Booking.Application.Services
{
    public class BookingServices : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public BookingServices(IBookingRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
            _messageBus = new RabbitMqMessageBus(_configuration["RabbitMQ:ConnectionString"] ?? "localhost");
        }

        public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        {
            // Validate business rules
            if (dto.ScheduledDate < DateTime.UtcNow)
            {
                throw new ArgumentException("Cannot book a past date");
            }

            //var existingBookings = await _repository.GetBookingsByDateAsync(dto.ScheduledDate.Date);

            var booking = dto.Adapt<Domain.Entities.Booking>();
            var created = await _repository.AddAsync(booking);
            
            await _messageBus.Publish("booking_created", new
            {
                EventName = "BookingCreated",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    BookingId = created.BookingId,
                    CustomerId = created.CustomerId,
                    ScheduledDate = created.ScheduledDate,
                    Status = created.Status.ToString()
                })
            });
            
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

        public async Task<IEnumerable<BookingListDto>> GetBookingsByDateAsync(
            DateTime date,
            int pageNumber = 1,
            int pageSize = 100)
        {
            var bookings = await _repository.GetBookingsByDateAsync(date, pageNumber, pageSize);
            return bookings.Adapt<IEnumerable<BookingListDto>>();
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid bookingId, string paymentStatus)
        {
            var validStatuses = new[] { "Unpaid", "Paid", "Partial", "Refunded" };
            if (!validStatuses.Contains(paymentStatus))
            {
                throw new ArgumentException($"Invalid payment status: {paymentStatus}");
            }

            return await _repository.UpdatePaymentStatusAsync(bookingId, paymentStatus);
        }


    }
}
