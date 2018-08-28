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

namespace RentApp.Controllers
{
    public class RatingsController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        public ApplicationUserManager UserManager { get; private set; }

        public RatingsController(IUnitOfWork unitOfWork, ApplicationUserManager applicationUserManager)
        {
            this.unitOfWork = unitOfWork;
            this.UserManager = applicationUserManager;
        }
        
        // GET: api/Ratings
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

        // PUT: api/Ratings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRating(int id, Rating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rating.Id)
            {
                return BadRequest();
            }

            try
            {
                unitOfWork.Ratings.Update(rating);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
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

        // POST: api/Ratings
        [ResponseType(typeof(Rating))]
        public async Task<IHttpActionResult> PostRating(CreateRatingBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            Rating rating = new Rating
            {
                UserId = user.Id,
                Value = model.Value
            };

            Service service = unitOfWork.Services.Get(model.ServiceId);
            if (service == null)
            {
                return NotFound();
            }

            service.Ratings.Add(rating);

            unitOfWork.Ratings.Add(rating);
            unitOfWork.Complete();

            return Ok(HttpStatusCode.OK);
        }

        private bool RatingExists(int id)
        {
            return unitOfWork.Ratings.Get(id) != null;
        }
    }
}