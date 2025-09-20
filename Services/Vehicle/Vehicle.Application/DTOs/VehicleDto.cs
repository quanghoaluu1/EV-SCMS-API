using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle.Application.DTOs
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public int OdometerKm { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
