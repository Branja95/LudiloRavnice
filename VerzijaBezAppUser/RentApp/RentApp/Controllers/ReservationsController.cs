using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Providers;
using RentApp.Results;
using RentApp.Persistance.UnitOfWork;
using RentApp.Hubs;
using System.Linq;
using System.Collections;
using RentApp.Helpers;
using System.Runtime.Remoting.Messaging;
using RentApp.Services;
using System.Web.Http.Description;
using System.Net;
using System.Data.Entity.Infrastructure;
using static RentApp.Models.ReservationBindingModel;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Reservations")]
    public class ReservationsController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public ReservationsController(ApplicationUserManager userManager, 
            IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        public ApplicationUserManager UserManager { get; private set; }

        // GET: api/Reservations
        public IEnumerable<Reservation> GetReservations()
        {
            return unitOfWork.Reservations.GetAll();
        }

        // GET: api/Reservations/5
        [ResponseType(typeof(Reservation))]
        public IHttpActionResult GetReservation(int id)
        {
            Reservation reservation = unitOfWork.Reservations.Get(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // PUT: api/Reservations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutReservation(int id, Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservation.Id)
            {
                return BadRequest();
            }

            try
            {
                unitOfWork.Reservations.Update(reservation);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
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

        // POST: api/Reservations/PostReservation
        [HttpPost]
        [Route("PostReservation")]
        [Authorize(Roles = "Admin, Manager, AppUser")]
        public IHttpActionResult PostReservation(CreateReservationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(unitOfWork.AccountsForApprove.Find(a => a.UserId == UserManager.FindById(User.Identity.GetUserId()).Id).Count() > 0)
            {
                return BadRequest("Your account is not approved.");
            }

            Vehicle vehicle = unitOfWork.Vehicles.Get(model.VehicleId);

            if(vehicle == null)
            {
                return BadRequest("Non-existent vehicle.");
            }

            if (!vehicle.IsAvailable)
            {
                return BadRequest("Vehicle is unavailable.");
            }

            if (model.ReservationStart >= model.ReservationEnd)
            {
                BadRequest("Reservation start date and time must be greater than reservation end date and time.");
            }

            if (model.ReservationStart < DateTime.Now)
            {
                BadRequest("Reservation start date and time must be greater than current date and time.");
            }

            if (model.ReservationEnd < DateTime.Now)
            {
                BadRequest("Reservation end date and time must be greater than current date and time.");
            }

            IEnumerable<Reservation> vehicleReservations = unitOfWork.Reservations.Find(r => r.Id == vehicle.Id);

            int numOfexcessiveReservations = vehicleReservations.Where(r => (r.ReservationStart <= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart <= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd)).Count();

            if(numOfexcessiveReservations > 0)
            {
                BadRequest("The vehicle was rented in a given period.");
            }

            BranchOffice rentBranchOffice = unitOfWork.BranchOffices.Get(model.RentBranchOfficeId);

            if (rentBranchOffice == null)
            {
                BadRequest("Non-existent rent branch office.");
            }


            BranchOffice returnBranchOffice = unitOfWork.BranchOffices.Get(model.ReturnBranchOfficeId);

            if (returnBranchOffice == null)
            {
                BadRequest("Non-existent return branch office.");
            }

            RAIdentityUser user = UserManager.FindById(User.Identity.GetUserId());

            Reservation reservation = new Reservation()
            {
                Vehicle = vehicle,
                UserId = user.Id,
                ReservationStart = model.ReservationStart,
                ReservationEnd = model.ReservationEnd,
                RentBranchOffice = rentBranchOffice,
                ReturnBranchOffice = returnBranchOffice
            };

            unitOfWork.Reservations.Add(reservation);
            unitOfWork.Complete();

            return Ok("Reservation successfully created.");
        }

        // DELETE: api/Reservations/5
        [ResponseType(typeof(Reservation))]
        public IHttpActionResult DeleteReservation(int id)
        {
            Reservation reservation = unitOfWork.Reservations.Get(id);
            if (reservation == null)
            {
                return NotFound();
            }

            unitOfWork.Reservations.Remove(reservation);
            unitOfWork.Complete();

            return Ok(reservation);
        }

        private bool ReservationExists(int id)
        {
            return unitOfWork.Reservations.Get(id) != null;
        }
    }
}