using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Persistance.Repository.Implementation
{
    public class RatingRepository : Repository<Rating, long>, IRatingRepository
    {
        private readonly DbContext _context;

        public RatingRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Rating> GetAll(int pageIndex, int pageSize)
        {
            return BookingDbContext.Ratings.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected BookingDbContext BookingDbContext { get { return context as BookingDbContext; } }
    }
}
