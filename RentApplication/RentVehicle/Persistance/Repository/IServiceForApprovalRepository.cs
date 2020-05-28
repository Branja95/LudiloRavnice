using RentVehicle.Models.Entities;
using System.Collections.Generic;

namespace RentVehicle.Persistance.Repository
{
    public interface IServiceForApprovalRepository : IRepository<ServiceForApproval, long>
    {
        IEnumerable<ServiceForApproval> GetAll(int pageIndex, int pageSize);
        long Count();
    }
}
