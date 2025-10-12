using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.IRepository
{
    public interface IBookingRepository
    {
        Task<Domain.Entities.Booking> AddAsync(Domain.Entities.Booking booking);
        Task<Domain.Entities.Booking?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Booking>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Domain.Entities.Booking?> UpdateAsync(Domain.Entities.Booking booking);
        Task<bool> DeleteAsync(Guid id);
        // Pagination support
        Task<int> GetTotalCountAsync();

        // Query methods
        Task<IEnumerable<Domain.Entities.Booking>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Domain.Entities.Booking>> GetByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<Domain.Entities.Booking>> GetByStaffIdAsync(Guid staffId);
        Task<IEnumerable<Domain.Entities.Booking>> GetByStatusAsync(BookingStatusEnum status, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Domain.Entities.Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Specific operations
        Task<bool> UpdateStatusAsync(Guid bookingId, BookingStatusEnum newStatus);
        Task<bool> AssignStaffAsync(Guid bookingId, Guid staffId);

        Task<IEnumerable<Domain.Entities.Booking>> GetBookingsByDateAsync(DateTime date, int pageNumber = 1, int pageSize = 100);

        Task<bool> UpdatePaymentStatusAsync(Guid bookingId, string paymentStatus);


    }
}
