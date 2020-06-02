using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Persistance.Repository.Implementation
{
    public class CommentRepository : Repository<Comment, long>, ICommentRepository
    {
        public CommentRepository(DbContext context) : base(context) { }

        public IEnumerable<Comment> GetAll(long serviceId)
        {
            return BookingDbContext.Comments.Where(x => x.ServiceId == serviceId);
        }

        public long Count()
        {
            return BookingDbContext.Set<Comment>().Count();
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
