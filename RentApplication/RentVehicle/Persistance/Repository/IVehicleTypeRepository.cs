using RentVehicle.Models.Entities;
using System.Collections.Generic;

namespace RentVehicle.Persistance.Repository
{
    public interface IVehicleTypeRepository : IRepository<VehicleType, long>
    {
        IEnumerable<VehicleType> GetAll(int pageIndex, int pageSize);
    }
}
