using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class VehicleRepository : Repository<Vehicle, long>, IVehicleRepository
    {
        public VehicleRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Vehicle> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.Vehicles.OrderBy(vehicle => vehicle.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RentVehicleDbContext RentVehicleDbContext { get { return context as RentVehicleDbContext; } }
    }
}
