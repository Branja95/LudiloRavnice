using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class VehicleRepository : Repository<Vehicle, long>, IVehicleRepository
    {
        private readonly DbContext _context;
        public VehicleRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public new IEnumerable<Vehicle> GetAll()
        {
            return RentVehicleDbContext.Vehicles.OrderBy(vehicle => vehicle.Id).Include(x => x.VehicleType);
        }

        public IEnumerable<Vehicle> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.Vehicles.OrderBy(vehicle => vehicle.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).Include(x => x.VehicleType);
        }

        protected RentVehicleDbContext RentVehicleDbContext { get { return context as RentVehicleDbContext; } }
    }
}
