using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentVehicle.Persistance.Repository.Implementation
{
    public class ServiceRepository : Repository<Service, long>, IServiceRepository
    {
        public ServiceRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Service> GetAll(int pageIndex, int pageSize)
        {
            return RentVehicleDbContext.Services.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RentVehicleDbContext RentVehicleDbContext { get { return context as RentVehicleDbContext; } }
    }
}
