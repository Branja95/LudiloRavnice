using RentVehicle.Models.Entities;
using System.Collections.Generic;

namespace RentVehicle.Persistance.Repository
{
    public interface IServiceRepository : IRepository<Service, long>
    {
        IEnumerable<Service> GetAll(int pageIndex, int pageSize);
    }
}
