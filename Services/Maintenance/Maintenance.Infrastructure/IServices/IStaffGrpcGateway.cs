using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.IServices
{
    public interface IStaffGrpcGateway
    {
        Task<StaffResponse> GetStaffById(Guid staffId);
    }

    public class StaffResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
