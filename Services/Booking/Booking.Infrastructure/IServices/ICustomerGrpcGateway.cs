using CustomerGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.IServices
{
    public interface ICustomerGrpcGateway
    {
        Task<GetCustomerByIdReply> GetCustomerById(Guid customerId);
    }
}
