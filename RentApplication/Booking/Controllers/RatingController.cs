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
using static Booking.Models.Bindings.RatingBindingModel;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private static object lockObjectForRaitings = new object();


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RatingController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> applicationUserManager)
        {
            _userManager = applicationUserManager;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Ratings/GetRatings
        [HttpGet]
        [Route("GetRatings")]
        [AllowAnonymous]
        public IEnumerable<Rating> GetRatings()
        {
            return _unitOfWork.Ratings.GetAll();
        }

        // GET: api/Ratings/5
        public IActionResult GetRating(int id)
        {
            Rating rating = _unitOfWork.Ratings.Get(id);
            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // PUT: api/Ratings/PutRating
        [HttpPut]
        [Route("PutRating")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public IActionResult PutRating([FromForm] int ratingId, EditRatingBindingModel rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Rating editRating = _unitOfWork.Ratings.Get(ratingId);
            if (editRating == null)
            {
                return BadRequest("Rating doesn't exist.");
            }

            editRating.Value = rating.Value;

            try
            {
                lock (lockObjectForRaitings)
                {
                    _unitOfWork.Ratings.Update(editRating);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/Ratings/PostRating
        [HttpPost]
        [Route("PostRating")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostRating(CreateRatingBindingModel model)
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
            if (!CanRating(user.Id, service))
            {
                return BadRequest("You can not rating on the service until your first renting is completed.");
            }

            Rating rating = new Rating
            {
                UserId = user.Id,
                Value = model.Value
            };

            try
            {
                service.Ratings.Add(rating);
                _unitOfWork.Ratings.Add(rating);
                _unitOfWork.Complete();
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Service successfully rated.");
        }

        private bool RatingExists(int id)
        {
            return _unitOfWork.Ratings.Get(id) != null;
        }

        private bool CanRating(string userId, Service service)
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