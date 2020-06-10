using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Booking.Models.Entities;
using Booking.Models.IdentityUsers;
using Booking.Persistance.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static Booking.Models.Bindings.RatingBindingModel;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private static object lockObjectForRaitings = new object();

        private readonly string _findUserEndpoint;
        private readonly IUnitOfWork _unitOfWork;

        private readonly string _serviceEndpoint;

        public RatingController(IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _serviceEndpoint = configuration["RentVehicleService:GetServiceEndpoint"];
            _findUserEndpoint = configuration["AccountService:FindUserEndpoint"];
        }


        [HttpDelete]
        [Route("DeleteRatingsForService")]
        public IActionResult DeleteComments([FromQuery] long serviceId)
        {
            List<Rating> ratings = _unitOfWork.Ratings.GetAll().Where(x => x.ServiceId == serviceId).ToList();
            try
            {
                _unitOfWork.Ratings.RemoveRange(ratings);
                _unitOfWork.Complete();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
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
            ApplicationUser user = await FindUser();
            if (user == null)
            {
                return Ok(true);
            }
            else
            {
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
                ApplicationUser user = await FindUser();
                if(user == null)
                {
                    return BadRequest();
                }
                else
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_serviceEndpoint + model.ServiceId).ConfigureAwait(true);
                        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            Service service = await httpResponseMessage.Content.ReadAsAsync<Service>().ConfigureAwait(false);
                            if (!CanUserRate(user.Id, service))
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

                        return BadRequest();
                    }
                }
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

        private async Task<ApplicationUser> FindUser()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_findUserEndpoint + User.FindFirstValue(ClaimTypes.NameIdentifier)).ConfigureAwait(true);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    ApplicationUser user = await httpResponseMessage.Content.ReadAsAsync<ApplicationUser>().ConfigureAwait(false);
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        private bool CanUserRate(string userId, Service service)
        {
            foreach (Vehicle vehicle in service.Vehicles)
            {
                if (_unitOfWork.Reservations.Find(r => r.VehicleId == vehicle.Id && r.UserId == userId && r.ReservationEnd < DateTime.Now).Any())
                {
                    return true;
                }
            }

            return false;
        }
    }
}