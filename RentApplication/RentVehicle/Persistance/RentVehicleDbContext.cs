using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using RentVehicle.Models.IdentityUsers;

namespace RentVehicle.Persistance
{
    public class RentVehicleDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<BranchOffice> BranchOffices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceForApproval> ServicesForApproval { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }

        public RentVehicleDbContext(DbContextOptions<RentVehicleDbContext> options) : base(options) { }
    }
}
