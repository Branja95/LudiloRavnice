using RentVehicle.Persistance.Repository;
using System;

namespace RentVehicle.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBranchOfficeRepository BranchOffices { get; set; }
        IServiceRepository Services { get; set; }
        IServiceForApprovalRepository ServicesForApproval { get; set; }
        IVehicleRepository Vehicles { get; set; }
        IVehicleTypeRepository VehicleTypes { get; set; }
        int Complete();
    }
}
