using System.Data.Entity;
using RentApp.Persistance.Repository;
using Unity.Attributes;

namespace RentApp.Persistance.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        [Dependency]
        public IServiceRepository Services { get; set; }
        [Dependency]
        public IAppUserRepository AppUsers { get; set; }
        [Dependency]
        public IBranchOfficeRepository BranchOffices { get; set; }
        [Dependency]
        public ICommentRepository Comments { get; set; }
        [Dependency]
        public IRatingRepository Ratings { get; set; }
        [Dependency]
        public IReservationRepository Reservations { get; set; }
        [Dependency]
        public IVehicleRepository Vehicles { get; set; }
        [Dependency]
        public IVehicleTypeRepository VehicleTypes { get; set; }

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