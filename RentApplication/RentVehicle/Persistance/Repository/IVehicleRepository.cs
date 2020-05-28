using RentVehicle.Models.Entities;
using System.Collections.Generic;

namespace RentVehicle.Persistance.Repository
{
    public interface IVehicleRepository : IRepository<Vehicle, long>
    {
        IEnumerable<Vehicle> GetAll(int pageIndex, int pageSize);
    }
}
