using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IAccountForApproveRepository : IRepository<AccountForApprove, long>
    {
        IEnumerable<AccountForApprove> GetAll(int pageIndex, int pageSize);

        long Count();
    }
}
