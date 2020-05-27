using AccountManaging.Persistance.Repository;
using Microsoft.EntityFrameworkCore;

namespace AccountManaging.Persistance.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ApplicationDbContext _context;

        private IAccountForApprovalRepository accountForApprovalRepository;
        private IServiceForApprovalRepository serviceForApprovalRepository;
        private IBanedManagerRepository banedManagerRepository;

        public IAccountForApprovalRepository AccountsForApproval
        {
            get
            {
                return accountForApprovalRepository = accountForApprovalRepository ?? new AccountForApprovalRepository(_context);
            }
            set
            {
                this.accountForApprovalRepository = value;
            }
        }
        public IServiceForApprovalRepository ServicesForApproval
        {
            get
            {
                return serviceForApprovalRepository = serviceForApprovalRepository ?? new ServiceForApprovalRepository(_context);
            }
            set
            {
                this.serviceForApprovalRepository = value;
            }
        }
        public IBanedManagerRepository BanedManagers
        {
            get
            {
                return banedManagerRepository = banedManagerRepository ?? new BanedManagerRepository(_context);
            }
            set
            {
                this.banedManagerRepository = value;
            }
        }

        public UnitOfWork(ApplicationDbContext context)
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
