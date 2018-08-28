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
using static RentApp.Models.RatingBindingModel;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Ratings")]
    public class RatingsController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        public ApplicationUserManager UserManager { get; private set; }

        public RatingsController(IUnitOfWork unitOfWork, ApplicationUserManager applicationUserManager)
        {
            this.unitOfWork = unitOfWork;
            this.UserManager = applicationUserManager;
        }

        // GET: api/Ratings/GetRatings
        [HttpGet]
        [Route("GetRatings")]
        [AllowAnonymous]
        public IEnumerable<Rating> GetRatings()
        {
            return unitOfWork.Ratings.GetAll();
        }

        // GET: api/Ratings/5
        [ResponseType(typeof(Rating))]
        public IHttpActionResult GetRating(int id)
        {
            Rating rating = unitOfWork.Ratings.Get(id);
            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // PUT: api/Ratings/PutRating
        [HttpPut]
        [Route("PutRating")]
        [Authorize(Roles = "Admin, Manager, AppUser")]
        public IHttpActionResult PutRating([FromUri] int ratingId, EditRatingBindingModel rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Rating editRating = unitOfWork.Ratings.Get(ratingId);
            if (editRating == null)
            {
                return BadRequest("Rating doesn't exist.");
            }

            editRating.Value = rating.Value;

            unitOfWork.Ratings.Update(editRating);
            unitOfWork.Complete();

            return Ok(HttpStatusCode.OK);
        }

        // POST: api/Ratings/PostRating
        [HttpPost]
        [Route("PostRating")]
        [Authorize(Roles = "Admin, Manager, AppUser")]
        public async Task<IHttpActionResult> PostRating(CreateRatingBindingModel model)
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

            //if(!CanRating(user.Id, service))
            //{
            //    return BadRequest("You can not rating on the service until your first renting is completed.");
            //}

            Rating rating = new Rating
            {
                UserId = user.Id,
                Value = model.Value
            };

            service.Ratings.Add(rating);

            unitOfWork.Ratings.Add(rating);
            unitOfWork.Complete();

            return Ok("Service successfully rated.");
        }

        private bool RatingExists(int id)
        {
            return unitOfWork.Ratings.Get(id) != null;
        }

        private bool CanRating(string userId, Service service)
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