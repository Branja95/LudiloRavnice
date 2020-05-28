using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Booking.Models.Entities;
using Booking.Models.IdentityUsers;
using Booking.Persistance.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Booking.Models.Bindings.CommentBindingModel;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private static object lockObjectForComments = new object();


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> applicationUserManager)
        {
            _userManager = applicationUserManager;
            _unitOfWork = unitOfWork;
        }


        // GET: api/Comments/GetComments
        [HttpGet]
        [AllowAnonymous]
        [Route("GetComments")]
        public IEnumerable<Comment> GetComments()
        {
            return _unitOfWork.Comments.GetAll();
        }

        // GET: api/Comments/5
        public IActionResult GetComment(int id)
        {
            Comment comment = _unitOfWork.Comments.Get(id);
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
        public async Task<IActionResult> UserName([FromForm] int commentId)
        {
            Comment comment = _unitOfWork.Comments.Get(commentId);
            if (comment == null)
            {
                return NotFound();
            }
            ApplicationUser applicationUser = _userManager.FindByIdAsync(comment.UserId).Result;

            return Ok(applicationUser.Email);
        }

        // PUT: api/Comments/PutComment
        [HttpPut]
        [Route("PutComment")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public IActionResult PutComment([FromForm] int commentId, EditCommentBindingModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment editComment = _unitOfWork.Comments.Get(commentId);
            if (editComment == null)
            {
                return BadRequest("Comment doesn't exist.");
            }

            editComment.Text = comment.Text;
            editComment.DateTime = DateTime.Now;

            try
            {
                _unitOfWork.Comments.Update(editComment);
                _unitOfWork.Complete();
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/Comments/PostComment
        [HttpPost]
        [Route("PostComment")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostComment(CreateCommentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //Service service = _unitOfWork.Services.Get(model.ServiceId);
            Service service = new Service();
            if (service == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                    _unitOfWork.Comments.Add(comment);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }
            return Ok("Service successfully commented.");
        }

        // DELETE: api/Comments/5
        public IActionResult DeleteComment(int id)
        {
            Comment comment = _unitOfWork.Comments.Get(id);
            if (comment == null)
            {
                return NotFound();
            }

            _unitOfWork.Comments.Remove(comment);
            _unitOfWork.Complete();

            return Ok(comment);
        }

        private bool CommentExists(int id)
        {
            return _unitOfWork.Comments.Get(id) != null;
        }

        private bool CanComment(string userId, Service service)
        {
            IEnumerable<Reservation> reservations = null;

            foreach (Vehicle vehicle in service.Vehicles)
            {
                reservations = _unitOfWork.Reservations.Find(r => r.Vehicle.Id == vehicle.Id && r.UserId == userId && r.ReservationEnd < DateTime.Now);

                if (reservations.Any())
                {
                    return true;
                }
            }

            return false;
        }
    }
}