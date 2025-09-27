using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle.Domain.Entities;

namespace Vehicle.Infrashtructure.IRepository
{
    public interface IVehicleRepository
    {
        Task<Domain.Entities.Vehicle> AddAsync(Domain.Entities.Vehicle vehicle);
        Task<Domain.Entities.Vehicle?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Vehicle>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Domain.Entities.Vehicle>> GetAllAsync();
        Task<Domain.Entities.Vehicle?> UpdateAsync(Domain.Entities.Vehicle vehicle);
        Task<bool> DeleteAsync(Guid id);
    }
}
