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
using static Booking.Models.Bindings.FeedbackBindingModel;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFeedbackController : ControllerBase
    {
        private static object lockObjectForUserFeedbacks = new object();

        private readonly IUnitOfWork _unitOfWork;

        private readonly string _serviceEndpoint;
        private readonly string _findUserEndpoint;

        public UserFeedbackController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;

            _serviceEndpoint = configuration["RentVehicleService:GetServiceEndpoint"];
            _findUserEndpoint = configuration["AccountService:FindUserEndpoint"];
        }

        [HttpGet]
        [Route("GetUserFeedbacks")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFeedbacks([FromQuery] long serviceId)
        {
            List<UserFeedback> userFeedbacks = _unitOfWork.UserFeedbacks.GetAll(serviceId).ToList();

            List<UserFeedbackBindingModel> feedbacks = new List<UserFeedbackBindingModel>();
            foreach(UserFeedback userFeedback in userFeedbacks)
            {
                ApplicationUser user = await FindUser(userFeedback.UserId);

                UserFeedbackBindingModel feedback = new UserFeedbackBindingModel
                {
                    Id = userFeedback.Id,
                    UserId = userFeedback.UserId,
                    ServiceId = userFeedback.ServiceId,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    Text = userFeedback.Text,
                    DateTime = userFeedback.DateTime,
                    Value = userFeedback.Value
                };

                feedbacks.Add(feedback);
            }
            
            return Ok(feedbacks);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("CanUserPostFeedback")]
        public async Task<IActionResult> CanUserPostFeedback([FromQuery] long serviceId)
        {
            ApplicationUser user = await FindUser();
            if(user == null)
            {
                return Ok(false);
            }
            else
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_serviceEndpoint + serviceId).ConfigureAwait(true);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        Service service = await httpResponseMessage.Content.ReadAsAsync<Service>().ConfigureAwait(false);
                        if (!CanUserGiveFeedback(user.Id, service))
                        {
                            return Ok(false);
                        }

                        UserFeedback userFeedback = _unitOfWork.UserFeedbacks.Find(com => com.UserId == user.Id).FirstOrDefault(x => x.ServiceId == serviceId);
                        if (userFeedback == null)
                        {
                            return Ok(true);
                        }

                        return Ok(false);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
        }


        [HttpPost]
        [Route("PostUserFeedback")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostUserFeedback([FromForm] CreateUserFeedbackBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                ApplicationUser user = await FindUser();
                if (user == null)
                {
                    return BadRequest();
                }
                else
                {
                    UserFeedback userFeedback = new UserFeedback
                    {
                        UserId = user.Id,
                        ServiceId = model.ServiceId,
                        Text = model.Text,
                        Value = model.Value,
                        DateTime = DateTime.Now
                    };

                    try
                    {
                        lock (lockObjectForUserFeedbacks)
                        {
                            _unitOfWork.UserFeedbacks.Add(userFeedback);
                            _unitOfWork.Complete();
                        }
                    }
                    catch (DBConcurrencyException)
                    {
                        return NotFound();
                    }

                    return Ok(_unitOfWork.UserFeedbacks.GetAll().ToList());
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

        private async Task<ApplicationUser> FindUser(string userId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_findUserEndpoint + userId).ConfigureAwait(true);
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

        private bool CanUserGiveFeedback(string userId, Service service)
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