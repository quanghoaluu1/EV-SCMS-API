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

        public async Task<Domain.Entities.Maintenance?> GetByIdAsync(Guid id)
        {
            return await _context.Maintenances
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);
        }

        public async Task<IEnumerable<Domain.Entities.Maintenance>> GetAllAsync()
        {
            return await _context.Maintenances.ToListAsync();
        }

        public async Task AddAsync(Domain.Entities.Maintenance maintenance)
        {
            await _context.Maintenances.AddAsync(maintenance);
        }

        public async Task UpdateAsync(Domain.Entities.Maintenance maintenance)
        {
            _context.Maintenances.Update(maintenance);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance != null)
            {
                _context.Maintenances.Remove(maintenance);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
