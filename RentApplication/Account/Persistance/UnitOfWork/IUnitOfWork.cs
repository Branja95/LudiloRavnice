using AccountManaging.Persistance.Repository;
using System;

namespace AccountManaging.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountForApprovalRepository AccountsForApproval { get; set; }
        IServiceForApprovalRepository ServicesForApproval { get; set; }
        IBanedManagerRepository BanedManagers { get; set; }
        int Complete();
    }
}
