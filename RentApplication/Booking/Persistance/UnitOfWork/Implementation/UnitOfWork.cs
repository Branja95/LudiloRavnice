using Booking.Persistance.Repository;
using Booking.Persistance.Repository.Implementation;
using Microsoft.EntityFrameworkCore;

namespace Booking.Persistance.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        private IUserFeedbackRepository userFeedbackRepository;
        private IReservationRepository reservationRepository;

        public IUserFeedbackRepository UserFeedbacks
        {
            get
            {
                return userFeedbackRepository = userFeedbackRepository ?? new UserFeedbackRepository(_context);
            }
            set
            {
                this.userFeedbackRepository = value;
            }
        }

        public IReservationRepository Reservations
        {
            get
            {
                return reservationRepository = reservationRepository ?? new ReservationRepository(_context);
            }
            set
            {
                this.reservationRepository = value;
            }
        }

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
