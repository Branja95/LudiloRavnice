using RentApp.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RentApp.Persistance.Repository
{
    public class CommentRepository : Repository<Comment, int>, ICommentRepository
    {
        public CommentRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Comment> GetAll(int pageIndex, int pageSize)
        {
            return RADBContext.Comments.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        protected RADBContext RADBContext { get { return context as RADBContext; } }
    }
}