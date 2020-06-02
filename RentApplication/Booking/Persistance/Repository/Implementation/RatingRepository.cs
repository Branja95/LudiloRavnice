using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Persistance.Repository.Implementation
{
    public class RatingRepository : Repository<Rating, long>, IRatingRepository
    {
        public RatingRepository(DbContext context) : base(context) { }

        public IEnumerable<Rating> GetAll(long serviceId)
        {
            return BookingDbContext.Ratings.Where(x=>x.ServiceId == serviceId);
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
