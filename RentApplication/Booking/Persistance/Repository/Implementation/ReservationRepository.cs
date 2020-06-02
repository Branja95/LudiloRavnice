using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Persistance.Repository.Implementation
{
    public class ReservationRepository : Repository<Reservation, long>, IReservationRepository
    {
        public ReservationRepository(DbContext context) : base(context) { }

        public IEnumerable<Reservation> GetAll(int pageIndex, int pageSize)
        {
            return BookingDbContext.Reservations.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected BookingDbContext BookingDbContext
        {
            get
            {
                return context as BookingDbContext;
            }
        }
    }
}
