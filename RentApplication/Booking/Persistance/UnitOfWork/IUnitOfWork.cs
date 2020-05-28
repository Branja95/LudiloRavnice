using Booking.Persistance.Repository;
using System;

namespace Booking.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICommentRepository Comments { get; set; }
        IRatingRepository Ratings { get; set; }
        IReservationRepository Reservations { get; set; }
        int Complete();
    }
}
