using Booking.Persistance.Repository;
using Booking.Persistance.Repository.Implementation;

namespace Booking.Persistance.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly BookingDbContext _context;


        private ICommentRepository commentRepository;
        private IRatingRepository ratingRepository;
        private IReservationRepository reservationRepository;

        public ICommentRepository Comments
        {
            get
            {
                return commentRepository = commentRepository ?? new CommentRepository(_context);
            }
            set
            {
                this.commentRepository = value;
            }
        }
        public IRatingRepository Ratings
        {
            get
            {
                return ratingRepository = ratingRepository ?? new RatingRepository(_context);
            }
            set
            {
                this.ratingRepository = value;
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
