using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class BranchOfficeRepository : Repository<BranchOffice, long>, IBranchOfficeRepository
    {
        public BranchOfficeRepository(DbContext context) : base(context) { }

        public IEnumerable<BranchOffice> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.BranchOffices.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RentVehicleDbContext RentVehicleDbContext { get { return context as RentVehicleDbContext; } }
    }
}
