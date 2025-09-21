using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public string Model { get; set; } = string.Empty;     
        public string VIN { get; set; } = string.Empty;      
        public string LicensePlate { get; set; } = string.Empty; 
        public int Year { get; set; }                         
        public int OdometerKm { get; set; }  
        public string Status { get; set; } = "Active";  //Active, Maintenance, Retired
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
