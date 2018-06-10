using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IBranchOfficeRepository : IRepository<BranchOffice, int>
    {
        IEnumerable<BranchOffice> GetAll(int pageIndex, int pageSize);
    }
}
