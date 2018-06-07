using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IReservationRepository
    {
        IEnumerable<Reservation> GetAll(int pageIndex, int pageSize);
    }
}
