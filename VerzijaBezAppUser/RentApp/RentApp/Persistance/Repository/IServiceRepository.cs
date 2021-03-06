﻿using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IServiceRepository : IRepository<Service, long>
    {
        IEnumerable<Service> GetAll(int pageIndex, int pageSize);

    }
}