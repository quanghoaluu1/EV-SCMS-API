using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.IRepository
{
    public interface IBookingRepository
    {
        Task<Domain.Entities.Booking?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Booking>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Domain.Entities.Booking>> GetAllAsync();
        Task<Domain.Entities.Booking> AddAsync(Domain.Entities.Booking booking);
        Task<Domain.Entities.Booking?> UpdateAsync(Domain.Entities.Booking booking);
        Task<bool> DeleteAsync(Guid id);
    }
}
