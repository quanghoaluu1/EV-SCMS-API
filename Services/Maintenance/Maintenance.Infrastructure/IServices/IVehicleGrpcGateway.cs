using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IServices
{
    public interface IVehicleGrpcGateway
    {
        Task<VehicleResponse> GetVehicleById(Guid vehicleId);
    }

    public class VehicleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
    }
}
