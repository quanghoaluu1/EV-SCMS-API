using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class StaffDbContext : DbContext
{
    public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options) { }

    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<StaffShift> StaffShifts { get; set; }
    public DbSet<Certification> Certifications { get; set; }
    public DbSet<PerformanceLog> PerformanceLogs { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Staff
        modelBuilder.Entity<Staff>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.StaffShifts)
            .WithOne(ss => ss.Staff)
            .HasForeignKey(ss => ss.StaffId);

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Certifications)
            .WithOne(c => c.Staff)
            .HasForeignKey(c => c.StaffId);

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.PerformanceLogs)
            .WithOne(p => p.Staff)
            .HasForeignKey(p => p.StaffId);

        // Shift
        modelBuilder.Entity<Shift>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Shift>()
            .HasMany(s => s.StaffShifts)
            .WithOne(ss => ss.Shift)
            .HasForeignKey(ss => ss.ShiftId);

        // StaffShift
        modelBuilder.Entity<StaffShift>()
            .HasKey(ss => ss.Id);

        // Certification
        modelBuilder.Entity<Certification>()
            .HasKey(c => c.Id);

        // PerformanceLog
        modelBuilder.Entity<PerformanceLog>()
            .HasKey(p => p.Id);

        // LeaveRequest
        modelBuilder.Entity<LeaveRequest>()
            .HasKey(l => l.Id);
    }
}
