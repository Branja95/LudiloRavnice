using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class VehicleRepository : Repository<Vehicle, long>, IVehicleRepository
    {
        public VehicleRepository(DbContext context) : base(context) { }

        public new Vehicle Get(long id)
        {
            return RentVehicleDbContext.Vehicles.Where(x => x.Id == id).Include(x => x.VehicleType).FirstOrDefault();
        }

        public new IEnumerable<Vehicle> GetAll()
        {
            return RentVehicleDbContext.Vehicles.OrderBy(vehicle => vehicle.Id).Include(x => x.VehicleType);
        }
        public IEnumerable<Vehicle> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.Vehicles.OrderBy(vehicle => vehicle.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).Include(x => x.VehicleType);
        }
        public new IEnumerable<Vehicle> Find(Expression<Func<Vehicle, bool>> predicate)
        {
            return RentVehicleDbContext.Vehicles.Where(predicate).Include(x => x.VehicleType);
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
