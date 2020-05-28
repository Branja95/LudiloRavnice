using Booking.Models.Entities;
using System.Collections.Generic;

namespace Booking.Persistance.Repository
{
    public interface IReservationRepository : IRepository<Reservation, long>
    {
        IEnumerable<Reservation> GetAll(int pageIndex, int pageSize);
    }
}
