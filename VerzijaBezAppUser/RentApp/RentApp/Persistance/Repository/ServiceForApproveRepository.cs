using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class ServiceForApproveRepository : Repository<ServiceForApprove, long>, IServiceForApproveRepository
    {
        public ServiceForApproveRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<ServiceForApprove> GetAll(int pageIndex, int pageSize)
        {
            return RADBContext.ServicesForApprove.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
  
        public long Count()
        {
            return RADBContext.Set<AccountForApprove>().Count();
        }

        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}