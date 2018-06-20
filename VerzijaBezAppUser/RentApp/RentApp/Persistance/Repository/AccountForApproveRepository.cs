using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class AccountForApproveRepository : Repository<AccountForApprove, long>, IAccountForApproveRepository
    {
        public AccountForApproveRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<AccountForApprove> GetAll(int pageIndex, int pageSize)
        {
            return RADBContext.AccountsForApprove.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public long Count()
        {
            return RADBContext.Set<AccountForApprove>().Count();
        }

        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}