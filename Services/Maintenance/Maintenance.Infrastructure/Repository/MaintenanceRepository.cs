using Maintenance.Domain.Enums;
using Maintenance.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.Repository
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly MaintenanceDbContext _context;

        public MaintenanceRepository(MaintenanceDbContext context)
        {
            _context = context;
        }

        // CRUD
        public async Task<Domain.Entities.Maintenance> AddAsync(Domain.Entities.Maintenance maintenance)
        {
            maintenance.MaintenanceId = Guid.NewGuid();
            maintenance.CreatedAt = DateTime.UtcNow;
            maintenance.UpdatedAt = DateTime.UtcNow;

            await _context.Maintenances.AddAsync(maintenance);
            await _context.SaveChangesAsync();
            return maintenance;
        }

        public async Task<Domain.Entities.Maintenance?> GetByIdAsync(Guid id)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Maintenance?> UpdateAsync(Domain.Entities.Maintenance maintenance)
        {
            var existing = await _context.Maintenances.FindAsync(maintenance.MaintenanceId);
            if (existing == null) return null;

            // Update fields
            existing.VehicleId = maintenance.VehicleId;
            existing.CustomerId = maintenance.CustomerId;
            existing.BookingId = maintenance.BookingId;
            existing.StaffId = maintenance.StaffId;
            existing.ServiceDate = maintenance.ServiceDate;
            existing.Odometer = maintenance.Odometer;
            existing.ServiceType = maintenance.ServiceType;
            existing.Description = maintenance.Description;
            existing.Status = maintenance.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null) return false;

            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Maintenances.CountAsync();
        }

        // QUERY METHODS 
        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .Where(m => m.BookingId == bookingId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetByCustomerIdAsync(
            Guid customerId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .Where(m => m.CustomerId == customerId)
                .OrderByDescending(m => m.ServiceDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .Where(m => m.VehicleId == vehicleId)
                .OrderByDescending(m => m.ServiceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetByStaffIdAsync(
            Guid staffId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .Where(m => m.StaffId == staffId)
                .OrderByDescending(m => m.ServiceDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetByStatusAsync(
            MaintenanceEnums status,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .Where(m => m.Status == status)
                .OrderBy(m => m.ServiceDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.Maintenances
                .AsNoTracking()
                .Where(m => m.ServiceDate >= startDate && m.ServiceDate <= endDate)
                .OrderBy(m => m.ServiceDate)
                .ToListAsync();
        }


        public async Task<bool> UpdateStatusAsync(Guid maintenanceId, MaintenanceEnums newStatus)
        {
            var maintenance = await _context.Maintenances.FindAsync(maintenanceId);
            if (maintenance == null) return false;

            maintenance.Status = newStatus;
            maintenance.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignStaffAsync(Guid maintenanceId, Guid staffId)
        {
            var maintenance = await _context.Maintenances.FindAsync(maintenanceId);
            if (maintenance == null) return false;

            maintenance.StaffId = staffId;
            maintenance.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
