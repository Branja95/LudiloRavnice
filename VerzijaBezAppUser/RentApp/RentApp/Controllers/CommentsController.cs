using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;
using System.Collections.Generic;
using System;
using static RentApp.Models.CommentBindingModel;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace RentApp.Controllers
{
    public class CommentsController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        public ApplicationUserManager UserManager { get; private set; }

        public CommentsController(IUnitOfWork unitOfWork, ApplicationUserManager applicationUserManager)
        {
            this.unitOfWork = unitOfWork;
            this.UserManager = applicationUserManager;
        }

        // GET: api/Comments
        public IEnumerable<Comment> GetComments()
        {
            return unitOfWork.Comments.GetAll();
        }

        // GET: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetComment(int id)
        {
            Comment comment = unitOfWork.Comments.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // GET: api/Comments/UserName
        [HttpGet]
        [Route("UserName")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UserName([FromUri] int commentId)
        {
            Comment comment = unitOfWork.Comments.Get(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(comment.UserId);
            
            return Ok(user.Email);
        }

        // PUT: api/Comments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComment(int id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.Id)
            {
                return BadRequest();
            }

            try
            {
                unitOfWork.Comments.Update(comment);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Comments/PostComment
        public async Task<IHttpActionResult> PostComment(CreateCommentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            Comment comment = new Comment
            {
                Text = model.Text,
                DateTime = DateTime.Now,
                UserId = user.Id
            };

            Service service = unitOfWork.Services.Get(model.ServiceId);
            if (service == null)
            {
                return NotFound();
            }

            service.Comments.Add(comment);

            unitOfWork.Comments.Add(comment);
            unitOfWork.Complete();
            
            return Ok(HttpStatusCode.OK);
        }

        // DELETE: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult DeleteComment(int id)
        {
            Comment comment = unitOfWork.Comments.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            unitOfWork.Comments.Remove(comment);
            unitOfWork.Complete();

            return Ok(comment);
        }

        private bool CommentExists(int id)
        {
            return unitOfWork.Comments.Get(id) != null;
        }
    }
}