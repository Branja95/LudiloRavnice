using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IServiceForApproveRepository : IRepository<ServiceForApprove, long>
    {
        IEnumerable<ServiceForApprove> GetAll(int pageIndex, int pageSize);

        long Count();
    }
}
