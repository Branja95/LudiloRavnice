using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Persistance.Repository.Implementation
{
    public class UserFeedbackRepository : Repository<UserFeedback, long>, IUserFeedbackRepository
    {
        public UserFeedbackRepository(DbContext context) : base(context) { }

        public IEnumerable<UserFeedback> GetAll(long serviceId)
        {
            return BookingDbContext.UserFeedbacks.Where(x => x.ServiceId == serviceId);
        }

        public long Count()
        {
            return BookingDbContext.Set<UserFeedback>().Count();
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
