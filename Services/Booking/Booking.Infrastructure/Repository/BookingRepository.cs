using Booking.Domain.Enums;
using Booking.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingServiceDbContext _context;

        public BookingRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Booking> AddAsync(Domain.Entities.Booking booking)
        {
            // Set timestamps
            booking.BookingId = Guid.NewGuid();
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking); 
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Bookings
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByCustomerIdAsync(
            Guid customerId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.ScheduledDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<Domain.Entities.Booking?> UpdateAsync(Domain.Entities.Booking booking)
        {
            var existing = await _context.Bookings.FindAsync(booking.BookingId);
            if (existing == null) return null;

            // Update only specific properties
            existing.VehicleId = booking.VehicleId;
            existing.StaffId = booking.StaffId;
            existing.ServiceType = booking.ServiceType;
            existing.ScheduledDate = booking.ScheduledDate;
            existing.Status = booking.Status;
            existing.Notes = booking.Notes;
            existing.TotalCost = booking.TotalCost;
            existing.PaymentStatus = booking.PaymentStatus;
            existing.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency conflicts
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByStatusAsync(
            BookingStatusEnum status,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Bookings
                .Where(b => b.Status == status)
                .OrderBy(b => b.ScheduledDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.ScheduledDate >= startDate && b.ScheduledDate <= endDate)
                .OrderBy(b => b.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.Bookings
                .Where(b => b.VehicleId == vehicleId)
                .OrderByDescending(b => b.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByStaffIdAsync(Guid staffId)
        {
            return await _context.Bookings
                .Where(b => b.StaffId == staffId)
                .OrderByDescending(b => b.ScheduledDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid bookingId, BookingStatusEnum newStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignStaffAsync(Guid bookingId, Guid staffId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.StaffId = staffId;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetBookingsByDateAsync
        (
            DateTime date,
            int pageNumber = 1,
            int pageSize = 100) 
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.ScheduledDate >= startOfDay && b.ScheduledDate <= endOfDay)
                .OrderBy(b => b.ScheduledDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid bookingId, string paymentStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.PaymentStatus = paymentStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
