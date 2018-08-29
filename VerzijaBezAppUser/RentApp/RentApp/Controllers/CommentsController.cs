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
using System.Data;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Comments")]
    public class CommentsController : ApiController
    {
        private static object lockObjectForComments = new object();

        private readonly IUnitOfWork unitOfWork;
        public ApplicationUserManager UserManager { get; private set; }

        public CommentsController(IUnitOfWork unitOfWork, ApplicationUserManager applicationUserManager)
        {
            this.unitOfWork = unitOfWork;
            this.UserManager = applicationUserManager;
        }

        // GET: api/Comments/GetComments
        [HttpGet]
        [AllowAnonymous]
        [Route("GetComments")]
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

        // PUT: api/Comments/PutComment
        [HttpPut]
        [Route("PutComment")]
        [Authorize(Roles = "Admin, Manager, AppUser")]
        public IHttpActionResult PutComment([FromUri] int commentId, EditCommentBindingModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment editComment = unitOfWork.Comments.Get(commentId);
            if (editComment == null)
            {
                return BadRequest("Comment doesn't exist.");
            }

            editComment.Text = comment.Text;
            editComment.DateTime = DateTime.Now;

            try
            {
                unitOfWork.Comments.Update(editComment);
                unitOfWork.Complete();
            }
            catch(DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok(HttpStatusCode.OK);
        }

        // POST: api/Comments/PostComment
        [HttpPost]
        [Route("PostComment")]
        [Authorize(Roles = "Admin, Manager, AppUser")]
        public async Task<IHttpActionResult> PostComment(CreateCommentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service service = unitOfWork.Services.Get(model.ServiceId);

            if (service == null)
            {
                return NotFound();
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (!CanComment(user.Id, service))
            {
                return BadRequest("You can not commenting on the service until your first renting is completed.");
            }

            Comment comment = new Comment
            {
                Text = model.Text,
                DateTime = DateTime.Now,
                UserId = user.Id
            };

            try
            {
                lock (lockObjectForComments)
                {
                    service.Comments.Add(comment);
                    unitOfWork.Comments.Add(comment);
                    unitOfWork.Complete();
                }
            }
            catch(DBConcurrencyException)
            {
                return NotFound();
            }
            return Ok("Service successfully commented.");
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

        private bool CanComment(string userId, Service service)
        {
            IEnumerable<Reservation> reservations = null;

            foreach (Vehicle vehicle in service.Vehicles)
            {
                reservations = unitOfWork.Reservations.Find(r => r.Vehicle.Id == vehicle.Id && r.UserId == userId && r.ReservationEnd < DateTime.Now);

                if (reservations.Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}