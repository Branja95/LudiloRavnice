using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IAppUserRepository
    {
        IEnumerable<AppUser> GetAll(int pageIndex, int pageSize);
    }
}
