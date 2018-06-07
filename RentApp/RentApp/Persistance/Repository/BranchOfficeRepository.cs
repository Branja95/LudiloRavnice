using RentApp.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RentApp.Persistance.Repository
{
    public class BranchOfficeRepository: Repository<BranchOffice, int>, IBranchOfficeRepository
    {
        public BranchOfficeRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<BranchOffice> GetAll(int pageIndex, int pageSize)
        {
            return RADBContext.BranchOffices.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}