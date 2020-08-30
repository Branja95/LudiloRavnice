using Booking.Models.Entities;
using System.Collections.Generic;

namespace Booking.Persistance.Repository
{
    public interface IUserFeedbackRepository : IRepository<UserFeedback, long>
    {
        IEnumerable<UserFeedback> GetAll(long serviceId);
    }
}
