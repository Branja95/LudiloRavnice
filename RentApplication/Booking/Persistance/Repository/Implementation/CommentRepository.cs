using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Persistance.Repository.Implementation
{
    public class CommentRepository : Repository<Comment, long>, ICommentRepository
    {
        private readonly DbContext _context;
        public CommentRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Comment> GetAll(int pageIndex, int pageSize)
        {
            return BookingDbContext.Comments.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public long Count()
        {
            return BookingDbContext.Set<Comment>().Count();
        }

        protected BookingDbContext BookingDbContext { get { return context as BookingDbContext; } }
    }
}
