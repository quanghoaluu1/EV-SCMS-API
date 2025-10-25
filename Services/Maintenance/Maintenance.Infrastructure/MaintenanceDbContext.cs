using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure
{
    public class MaintenanceDbContext : DbContext
    {
        public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
        {
        }
        public DbSet<Domain.Entities.Maintenance> Maintenances { get; set; } = null!;
        public DbSet<Domain.Entities.MaintenanceInventoryItem> MaintenanceInventoryItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Maintenance entity
            modelBuilder.Entity<Domain.Entities.Maintenance>(entity =>
            {
                entity.HasKey(e => e.MaintenanceId);
                entity.Property(e => e.MaintenanceId).ValueGeneratedOnAdd();
            });

            // Configure MaintenanceInventoryItem entity
            modelBuilder.Entity<Domain.Entities.MaintenanceInventoryItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Configure relationship
                entity.HasOne(mi => mi.Maintenance)
                    .WithMany(m => m.InventoryItems)
                    .HasForeignKey(mi => mi.MaintenanceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
