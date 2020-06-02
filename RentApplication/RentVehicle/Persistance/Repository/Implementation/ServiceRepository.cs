using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class ServiceRepository : Repository<Service, long>, IServiceRepository
    {
        public ServiceRepository(DbContext context) : base(context) { }

        public new Service Get(long id)
        {
            return RentVehicleDbContext.Services.Where(service => service.Id == id)
                .Include(service => service.BranchOfficces)
                .Include(service => service.Vehicles)
                .ThenInclude(vehicle => vehicle.VehicleType)
                .FirstOrDefault();
        }

        public new IEnumerable<Service> GetAll()
        {
            return RentVehicleDbContext.Services
                .Include(service => service.BranchOfficces)
                .Include(service => service.Vehicles);
        }


        public IEnumerable<Service> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.Services.Skip((pageIndex - 1) * pageSize).Take(pageSize).Include(x => x.Vehicles);
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
