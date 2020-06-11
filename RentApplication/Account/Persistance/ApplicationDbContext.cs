using AccountManaging.Models.Entities;
using AccountManaging.Models.IdentityUsers;
using AccountManaging.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountManaging.Persistance
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<AccountForApproval> AccountsForApproval { get; set; }
        public DbSet<BanedManager> BanedManagers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
