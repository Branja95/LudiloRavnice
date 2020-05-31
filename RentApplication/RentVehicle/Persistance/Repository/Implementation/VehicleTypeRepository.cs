using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class VehicleTypeRepository : Repository<VehicleType, long>, IVehicleTypeRepository
    {
        public VehicleTypeRepository(DbContext context) : base(context) { }

        public IEnumerable<VehicleType> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.VehicleTypes.Skip((pageIndex - 1) * pageSize).Take(pageSize);
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
