using Microsoft.EntityFrameworkCore;
using RentVehicle.Persistance.Repository;

namespace RentVehicle.Persistance.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public IBranchOfficeRepository BranchOffices { get; set; }
        public IServiceRepository Services { get; set; }
        public IServiceForApprovalRepository ServicesForApproval { get; set; }
        public IVehicleRepository Vehicles { get; set; }
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
