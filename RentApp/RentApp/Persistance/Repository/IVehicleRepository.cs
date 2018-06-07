using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IVehicleRepository
    {
        IEnumerable<Vehicle> GetAll(int pageIndex, int pageSize);
    }
}
