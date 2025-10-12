using StaffGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IServices
{
    public interface IStaffGrpcGateway
    {
        Task<GetStaffByIdReply> GetStaffById(Guid staffId);
    }
}
