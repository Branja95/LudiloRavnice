using RentApp.Persistance.Repository;
using System;

namespace RentApp.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAppUserRepository AppUsers { get; set; }
        IBranchOfficeRepository BranchOffices { get; set; }
        ICommentRepository Comments { get; set; }
        IRatingRepository Ratings { get; set; }
        IReservationRepository Reservations { get; set; }
        IServiceRepository Services { get; set; }
        IVehicleRepository Vehicles { get; set; }
        IVehicleTypeRepository VehicleTypes { get; set; }

        int Complete();
    }
}
