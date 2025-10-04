
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }
        public DbSet<Domain.Entities.Notification> Notifications { get; set; } = null!;
    }
}
