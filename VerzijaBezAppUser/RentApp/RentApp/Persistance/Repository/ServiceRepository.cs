﻿using RentApp.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RentApp.Persistance.Repository
{
    public class ServiceRepository : Repository<Service, long>, IServiceRepository
    {
        public ServiceRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Service> GetAll(int pageIndex, int pageSize)
        {
            return RADBContext.Services.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}