using AccountManaging.Models.Entities;
using System.Collections.Generic;

namespace AccountManaging.Persistance.Repository
{
    public interface IAccountForApprovalRepository : IRepository<AccountForApproval, long>
    {
        IEnumerable<AccountForApproval> GetAll(int pageIndex, int pageSize);
        long Count();
    }
}