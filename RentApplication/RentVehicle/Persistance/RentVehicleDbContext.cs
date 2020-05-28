using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;

namespace RentVehicle.Persistance
{
    public class RentVehicleDbContext : IdentityDbContext
    {
        public DbSet<BranchOffice> BranchOffices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceForApproval> ServicesForApproval { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }

        public RentVehicleDbContext(DbContextOptions<RentVehicleDbContext> options) : base(options) { }
    }
}
