using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IVehicleTypeRepository
    {
        IEnumerable<VehicleType> GetAll(int pageIndex, int pageSize);
    }
}
