using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure
{
    public class BookingServiceDbContext : DbContext
    {
        public BookingServiceDbContext(DbContextOptions<BookingServiceDbContext> options) : base(options)
        {
        }
        public DbSet<Domain.Entities.Booking> Bookings { get; set; } = null!;
    }
}
