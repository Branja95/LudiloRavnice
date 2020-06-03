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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        private readonly string serviceUrl;

        public CommentController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> applicationUserManager,
            IConfiguration configuration)
        {
            _userManager = applicationUserManager;
            _unitOfWork = unitOfWork;

            serviceUrl = configuration["RentVehicleService:ServiceUrl"];
        }


        [HttpGet]
        [Route("GetComment")]
        [AllowAnonymous]
        public IActionResult GetComment([FromQuery] int id)
        {
            Comment comment = _unitOfWork.Comments.Get(id);
            if (comment == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(comment);
            }
        }


        [HttpGet]
        [Route("GetComments")]
        [AllowAnonymous]
        public IEnumerable<Comment> GetComments([FromQuery] long serviceId)
        {
            return _unitOfWork.Comments.GetAll(serviceId);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("HasUserCommented")]
        public async Task<IActionResult> HasUserCommented([FromQuery] long serviceId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return Ok(true);
            }

            Comment comment = _unitOfWork.Comments.Find(com => com.UserId == user.Id).FirstOrDefault(x=>x.ServiceId == serviceId);
            if (comment == null)
            {
                return Ok(false);
            }
            else
            {
                return Ok(true);
            }
        }


        [HttpPost]
        [Route("PostComment")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostComment([FromForm] CreateCommentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(serviceUrl + model.ServiceId).ConfigureAwait(true);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        Service service = await httpResponseMessage.Content.ReadAsAsync<Service>().ConfigureAwait(false);
                        if (!CanUserComment(user.Id, service))
                        {
                            return BadRequest();
                        }

                        Comment comment = new Comment
                        {
                            UserId = user.Id,
                            ServiceId = model.ServiceId,
                            Text = model.Text,
                            DateTime = DateTime.Now
                        };

                        try
                        {
                            lock (lockObjectForComments)
                            {
                                _unitOfWork.Comments.Add(comment);
                                _unitOfWork.Complete();
                            }
                        }
                        catch (DBConcurrencyException)
                        {
                            return NotFound();
                        }

                        return Ok();
                    }
                    return NotFound();
                }
            }
        }


        [HttpPut]
        [Route("PutComment")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public IActionResult PutComment([FromForm] EditCommentBindingModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Comment editComment = _unitOfWork.Comments.Get(comment.Id);
                if (editComment == null)
                {
                    return BadRequest();
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
        }


        private bool CanUserComment(string userId, Service service)
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