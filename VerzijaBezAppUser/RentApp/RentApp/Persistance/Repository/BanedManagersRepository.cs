using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class BanedManagersRepository : Repository<BanedManager, long>, IBanedManagersRepository
    {
        public BanedManagersRepository(DbContext context) : base(context) { }
        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}