using Booking.Models.Entities;
using System.Collections.Generic;

namespace Booking.Persistance.Repository
{
    public interface IRatingRepository : IRepository<Rating, long>
    {
        IEnumerable<Rating> GetAll(int pageIndex, int pageSize);
    }
}
