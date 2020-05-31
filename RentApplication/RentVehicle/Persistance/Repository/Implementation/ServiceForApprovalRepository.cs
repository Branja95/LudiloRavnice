using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class ServiceForApprovalRepository : Repository<ServiceForApproval, long>, IServiceForApprovalRepository
    {
        public ServiceForApprovalRepository(DbContext context) : base(context) { }
        
        public IEnumerable<ServiceForApproval> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.ServicesForApproval.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public long Count()
        {
            return RentVehicleDbContext.Set<ServiceForApproval>().Count();
        }

        protected RentVehicleDbContext RentVehicleDbContext
        {
            get
            {
                return _context as RentVehicleDbContext;
            }
        }
    }
}
