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


        [HttpGet]
        [Route("GetRating")]
        [AllowAnonymous]
        public IActionResult GetRating([FromQuery] int id)
        {
            Rating rating = _unitOfWork.Ratings.Get(id);
            if (rating == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(rating);
            }
        }


        [HttpGet]
        [Route("GetRatings")]
        [AllowAnonymous]
        public IEnumerable<Rating> GetRatings([FromQuery] long serviceId)
        {
            return _unitOfWork.Ratings.GetAll(serviceId);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("HasUserRated")]
        public async Task<IActionResult> HasUserRated([FromQuery] long serviceId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return Ok(true);
            }

            Rating rating = _unitOfWork.Ratings.Find(rate => rate.UserId == user.Id).FirstOrDefault(x => x.ServiceId == serviceId);
            if (rating == null)
            {
                return Ok(false);
            }
            else
            {
                return Ok(true);
            }
        }


        [HttpPost]
        [Route("PostRating")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostRating(CreateRatingBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!CanRating(user.Id))
                {
                    return BadRequest();
                }

                Rating rating = new Rating
                {
                    UserId = user.Id,
                    ServiceId = model.ServiceId,
                    Value = model.Value
                };

                try
                {
                    _unitOfWork.Ratings.Add(rating);
                    _unitOfWork.Complete();
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }

                return Ok();
            }
        }


        [HttpPut]
        [Route("PutRating")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public IActionResult PutRating([FromForm] EditRatingBindingModel rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {

                Rating editRating = _unitOfWork.Ratings.Get(rating.Id);
                if (editRating == null)
                {
                    return BadRequest("Rating doesn't exist.");
                }
                else
                {
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
            }
        }


        private bool CanRating(string userId)
        {
            /*
            IEnumerable<Reservation> reservations = null;

            foreach (Vehicle vehicle in service.Vehicles)
            {
                reservations = _unitOfWork.Reservations.Find(r => r.Vehicle.Id == vehicle.Id && r.UserId == userId && r.ReservationEnd < DateTime.Now);
                if (reservations.Any())
                {
                    return true;
                }
            }
            */
            return true;
        }
    }
}