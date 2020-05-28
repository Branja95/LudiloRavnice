using RentVehicle.Models.Entities;
using System.Collections.Generic;

namespace RentVehicle.Persistance.Repository
{
    public interface IBranchOfficeRepository : IRepository<BranchOffice, long>
    {
        IEnumerable<BranchOffice> GetAll(int pageIndex, int pageSize);
    }
}
