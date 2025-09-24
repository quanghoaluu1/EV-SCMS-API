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
            await _context.bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var booking = await _context.bookings.FindAsync(id);
            if (booking == null) return false;

            _context.bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetAllAsync()
        {
            return await _context.bookings.ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.bookings
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Booking?> GetByIdAsync(Guid id)
        {
            return await _context.bookings.FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<Domain.Entities.Booking> UpdateAsync(Domain.Entities.Booking booking)
        {
            var existing = await _context.bookings.FindAsync(booking.BookingId);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

    }
}
