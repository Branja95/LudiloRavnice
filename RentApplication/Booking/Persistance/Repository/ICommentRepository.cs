using Booking.Models.Entities;
using System.Collections.Generic;

namespace Booking.Persistance.Repository
{
    public interface ICommentRepository : IRepository<Comment, long>
    {
        IEnumerable<Comment> GetAll(int pageIndex, int pageSize);
    }
}
