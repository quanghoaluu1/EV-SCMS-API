using Maintenance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IRepository
{
    public interface IMaintenanceRepository
    {
        Task<Domain.Entities.Maintenance?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Maintenance>> GetAllAsync();
        Task AddAsync(Domain.Entities.Maintenance maintenance);
        Task UpdateAsync(Domain.Entities.Maintenance maintenance);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
