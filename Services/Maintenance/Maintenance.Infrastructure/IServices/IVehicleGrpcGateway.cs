using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleGrpc;

namespace Maintenance.Infrastructure.IServices
{
    public interface IVehicleGrpcGateway
    {
        Task<GetVehicleByIdReply> GetVehicleById(Guid vehicleId);
    }
}
