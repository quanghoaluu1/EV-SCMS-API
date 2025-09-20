using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure;

public class AuthServiceDbContext : DbContext
{
    public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> User { get; set; } = null!;
}