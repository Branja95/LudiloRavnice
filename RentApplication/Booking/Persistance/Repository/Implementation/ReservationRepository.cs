using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Persistance.Repository.Implementation
{
    public class ReservationRepository : Repository<Reservation, long>, IReservationRepository
    {
        private readonly DbContext _context;

        public ReservationRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Reservation> GetAll(int pageIndex, int pageSize)
        {
            return BookingDbContext.Reservations.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected BookingDbContext BookingDbContext { get { return context as BookingDbContext; } }
    }
}
