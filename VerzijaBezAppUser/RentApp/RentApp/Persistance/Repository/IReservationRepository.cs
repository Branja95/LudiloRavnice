using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IReservationRepository : IRepository<Reservation, int>
    {
        IEnumerable<Reservation> GetAll(int pageIndex, int pageSize);
    }
}
