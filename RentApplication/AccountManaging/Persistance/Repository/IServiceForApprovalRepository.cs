using AccountManaging.Models.Entities;
using System.Collections.Generic;

namespace AccountManaging.Persistance.Repository
{
    public interface IServiceForApprovalRepository : IRepository<ServiceForApproval, long>
    {
        IEnumerable<ServiceForApproval> GetAll(int pageIndex, int pageSize);

        long Count();
    }
}
