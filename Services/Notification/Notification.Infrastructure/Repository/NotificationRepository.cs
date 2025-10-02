using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Domain.Entities.Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task AddAsync(Domain.Entities.Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task UpdateAsync(Domain.Entities.Notification notification)
        {
            _context.Notifications.Update(notification);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
