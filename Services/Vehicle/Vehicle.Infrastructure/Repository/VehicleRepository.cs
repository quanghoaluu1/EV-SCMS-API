using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle.Infrashtructure.IRepository;
using Vehicle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Vehicle.Infrashtructure.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VehicleServiceDbContext _context;
        public VehicleRepository(VehicleServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Vehicle> AddAsync(Domain.Entities.Vehicle vehicle)
        {
            _context.vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var vehicle = await _context.vehicles.FindAsync(id);
            if (vehicle == null) return false;

            _context.vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Domain.Entities.Vehicle>> GetAllAsync()
        {
            return await _context.vehicles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Vehicle>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.vehicles
                .AsNoTracking()
                .Where(v => v.UserId == customerId)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Vehicle?> GetByIdAsync(Guid id)
        {
            return await _context.vehicles.FindAsync(id);
        }

        public async Task<Domain.Entities.Vehicle?> UpdateAsync(Domain.Entities.Vehicle vehicle)
        {
            var existing = await _context.vehicles.FindAsync(vehicle.Id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(vehicle);
            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
