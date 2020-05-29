using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class VehicleTypeRepository : Repository<VehicleType, long>, IVehicleTypeRepository
    {
        private readonly DbContext _context;

        public VehicleTypeRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<VehicleType> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.VehicleTypes.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RentVehicleDbContext RentVehicleDbContext
        {
            get { return context as RentVehicleDbContext; }
        }
    }
}
