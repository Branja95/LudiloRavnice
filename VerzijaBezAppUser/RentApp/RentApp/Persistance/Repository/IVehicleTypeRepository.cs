using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IVehicleTypeRepository : IRepository<VehicleType, int>
    {
        IEnumerable<VehicleType> GetAll(int pageIndex, int pageSize);
    }
}
