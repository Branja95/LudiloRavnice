using Booking.Persistance.Repository;
using System;

namespace Booking.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserFeedbackRepository UserFeedbacks { get; set; }
        IReservationRepository Reservations { get; set; }
        int Complete();
    }
}
