using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IServices
{
    public interface ICustomerGrpcGateway
    {
        Task<CustomerResponse> GetCustomerById(Guid customerId);
    }

    public class CustomerResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
