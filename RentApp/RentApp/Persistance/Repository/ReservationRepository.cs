using RentApp.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RentApp.Persistance.Repository
{
    public class ReservationRepository : Repository<Reservation, int>, IReservationRepository
    {
        public ReservationRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Reservation> GetAll(int pageIndex, int pageSize)
        {
            return RADBContext.Reservations.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}