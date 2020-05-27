
using AccountManaging.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountManaging.Persistance.Repository
{
    public class BanedManagerRepository : Repository<BanedManager, long>, IBanedManagerRepository
    {
        public BanedManagerRepository(DbContext context) : base(context) { }

        protected ApplicationDbContext ApplicationDbContext { get { return context as ApplicationDbContext; } }
    }
}
