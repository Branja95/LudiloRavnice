using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IBranchOfficeRepository
    {
        IEnumerable<BranchOffice> GetAll(int pageIndex, int pageSize);
    }
}
