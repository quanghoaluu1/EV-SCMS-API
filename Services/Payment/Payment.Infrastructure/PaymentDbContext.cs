using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;

namespace Payment.Infrastructure;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext (DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }
    
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>()
            .HasKey(p => p.Id);
    }
    
}