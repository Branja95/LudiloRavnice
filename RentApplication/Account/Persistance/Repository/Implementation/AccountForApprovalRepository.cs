using AccountManaging.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountManaging.Persistance.Repository
{
    public class AccountForApprovalRepository : Repository<AccountForApproval, long>, IAccountForApprovalRepository
    {
        private readonly DbContext _context;
        public AccountForApprovalRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<AccountForApproval> GetAll(int pageIndex, int pageSize)
        {
            return ApplicationDbContext.AccountsForApproval.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public long Count()
        {
            return ApplicationDbContext.Set<AccountForApproval>().Count();
        }

        protected ApplicationDbContext ApplicationDbContext { get { return context as ApplicationDbContext; }  }
    }
}
