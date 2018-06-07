using RentApp.Models.Entities;
using System.Collections.Generic;

namespace RentApp.Persistance.Repository
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> GetAll(int pageIndex, int pageSize);
    }
}
