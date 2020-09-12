using RentVehicle.Models.Entities;
using System.Collections.Generic;

namespace RentVehicle.Persistance.Repository
{
    public interface IServiceForApprovalRepository : IRepository<ServiceForApproval, long>
    {
        new ServiceForApproval Get(long serviceId);
        IEnumerable<ServiceForApproval> GetAll(int pageIndex, int pageSize);
        long Count();
    }
}
