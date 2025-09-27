using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Vehicle.Infrashtructure
{
    public class VehicleServiceDbContext : DbContext
    {
        public VehicleServiceDbContext(DbContextOptions<VehicleServiceDbContext> options) : base(options)
        {

        }
        public DbSet<Vehicle.Domain.Entities.Vehicle> vehicles { get; set; } = null!;
    }
}
