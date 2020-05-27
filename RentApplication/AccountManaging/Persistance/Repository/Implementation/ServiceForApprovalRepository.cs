using AccountManaging.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountManaging.Persistance.Repository
{
    public class ServiceForApprovalRepository : Repository<ServiceForApproval, long>, IServiceForApprovalRepository
    {
        public ServiceForApprovalRepository(DbContext context) : base(context) { }


        public IEnumerable<ServiceForApproval> GetAll(int pageIndex, int pageSize)
        {
            return ApplicationDbContext.ServicesForApproval.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public long Count()
        {
            return ApplicationDbContext.Set<ServiceForApproval>().Count();
        }

        protected ApplicationDbContext ApplicationDbContext { get { return context as ApplicationDbContext; } }
    }
}
