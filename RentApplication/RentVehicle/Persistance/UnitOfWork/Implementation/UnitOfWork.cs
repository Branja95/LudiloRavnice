using Microsoft.EntityFrameworkCore;
using RentVehicle.Persistance.Repository;
using RentVehicle.Persistance.Repository.Implementation;

namespace RentVehicle.Persistance.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        private IBranchOfficeRepository branchOfficeRepository;
        private IServiceRepository serviceRepository;
        private IServiceForApprovalRepository servicesForApproval;
        private IVehicleRepository vehicleRepository;
        private IVehicleTypeRepository vehicleTypeRepository;

        public IBranchOfficeRepository BranchOffices
        {
            get
            {
                return branchOfficeRepository = branchOfficeRepository ?? new BranchOfficeRepository(_context);
            }
            set
            {
                this.branchOfficeRepository = value;
            }
        }

        public IServiceRepository Services
        {
            get
            {
                return serviceRepository = serviceRepository ?? new ServiceRepository(_context);
            }
            set
            {
                this.serviceRepository = value;
            }
        }

        public IServiceForApprovalRepository ServicesForApproval
        {
            get
            {
                return servicesForApproval = servicesForApproval ?? new ServiceForApprovalRepository(_context);
            }
            set
            {
                this.servicesForApproval = value;
            }
        }

        public IVehicleRepository Vehicles
        {
            get
            {
                return vehicleRepository = vehicleRepository ?? new VehicleRepository(_context);
            }
            set
            {
                this.vehicleRepository = value;
            }
        }

        public IVehicleTypeRepository VehicleTypes
        {
            get
            {
                return vehicleTypeRepository = vehicleTypeRepository ?? new VehicleTypeRepository(_context);
            }
            set
            {
                this.vehicleTypeRepository = value;
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
