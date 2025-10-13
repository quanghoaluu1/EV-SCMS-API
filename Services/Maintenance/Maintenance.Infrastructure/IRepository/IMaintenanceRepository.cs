using Maintenance.Domain.Entities;
using Maintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IRepository
{
    public interface IMaintenanceRepository
    {
        Task<Domain.Entities.Maintenance> AddAsync(Domain.Entities.Maintenance maintenance);
        Task<Domain.Entities.Maintenance?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Domain.Entities.Maintenance?> UpdateAsync(Domain.Entities.Maintenance maintenance);
        Task<bool> DeleteAsync(Guid id);

        // Pagination
        Task<int> GetTotalCountAsync();

        // Query methods 
        Task<IEnumerable<Domain.Entities.Maintenance>> GetByBookingIdAsync(Guid bookingId);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetByStaffIdAsync(Guid staffId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetByStatusAsync(MaintenanceEnums status, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Specific operations
        Task<bool> UpdateStatusAsync(Guid maintenanceId, MaintenanceEnums newStatus);
        Task<bool> AssignStaffAsync(Guid maintenanceId, Guid staffId);
    }
}
