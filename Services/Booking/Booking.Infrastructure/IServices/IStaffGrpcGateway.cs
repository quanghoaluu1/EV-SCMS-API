using StaffGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.IServices
{
    public interface IStaffGrpcGateway
    {
        Task<GetStaffByIdReply> GetStaffById(Guid staffId);
    }
}
