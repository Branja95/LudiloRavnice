﻿using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface IRatingRepository
    {
        IEnumerable<Rating> GetAll(int pageIndex, int pageSize);
    }
}
