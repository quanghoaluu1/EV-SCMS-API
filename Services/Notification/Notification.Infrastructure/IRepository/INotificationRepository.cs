using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.IRepository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Domain.Entities.Notification>> GetAllAsync();
        Task<Domain.Entities.Notification?> GetByIdAsync(Guid id);
        Task AddAsync(Domain.Entities.Notification notification);
        Task UpdateAsync(Domain.Entities.Notification notification);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
