using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Booking.Models.Entities;
using Booking.Models.IdentityUsers;
using Booking.Persistance.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Booking.Models.Bindings.ReservationBindingmodel;

namespace Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private static object lockObjectForReservations = new object();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ReservationController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> applicationUserManager)
        {
            _userManager = applicationUserManager;
            _unitOfWork = unitOfWork;
        }


        // GET: api/Reservations
        public IEnumerable<Reservation> GetReservations()
        {
            return _unitOfWork.Reservations.GetAll();
        }

        // GET: api/Reservations/5
        public IActionResult GetReservation(int id)
        {
            Reservation reservation = _unitOfWork.Reservations.Get(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // PUT: api/Reservations/5
        public IActionResult PutReservation(int id, Reservation reservation)
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
                _unitOfWork.Reservations.Update(reservation);
                _unitOfWork.Complete();
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

            //return StatusCode(HttpStatusCode.NoContent);
            return BadRequest();
        }

        // POST: api/Reservations/PostReservation
        [HttpPost]
        [Route("PostReservation")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostReservationAsync(CreateReservationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (_unitOfWork.AccountsForApprove.Find(a => a.UserId == UserManager.FindById(User.Identity.GetUserId()).Id).Count() > 0)
            //{
            //    return BadRequest("Your account is not approved.");
            //}

            // Vehicle vehicle = _unitOfWork.Vehicles.Get(model.VehicleId);
            Vehicle vehicle = new Vehicle();
            if (vehicle == null)
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

            //BranchOffice rentBranchOffice = _unitOfWork.BranchOffices.Get(model.RentBranchOfficeId);
            BranchOffice rentBranchOffice = new BranchOffice();
            if (rentBranchOffice == null)
            {
                BadRequest("Non-existent rent branch office.");
            }


            //BranchOffice returnBranchOffice = _unitOfWork.BranchOffices.Get(model.ReturnBranchOfficeId);
            BranchOffice returnBranchOffice = new BranchOffice();
            if (returnBranchOffice == null)
            {
                BadRequest("Non-existent return branch office.");
            }

            //lock (lockObjectForReservations)
            //{
                IEnumerable<Reservation> vehicleReservations = _unitOfWork.Reservations.Find(r => r.Id == vehicle.Id);

                int numOfexcessiveReservations = vehicleReservations.Where(r => (r.ReservationStart <= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart <= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd)).Count();

                if (numOfexcessiveReservations > 0)
                {
                    BadRequest("The vehicle was rented in a given period.");
                }

                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Reservation reservation = new Reservation()
                {
                    Vehicle = vehicle,
                    UserId = user.Id,
                    ReservationStart = model.ReservationStart,
                    ReservationEnd = model.ReservationEnd,
                    RentBranchOffice = rentBranchOffice,
                    ReturnBranchOffice = returnBranchOffice
                };

                try
                {
                    _unitOfWork.Reservations.Add(reservation);
                    _unitOfWork.Complete();
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }

            //}

            return Ok("Reservation successfully created.");
        }

        // DELETE: api/Reservations/5
        public IActionResult DeleteReservation(int id)
        {
            Reservation reservation = _unitOfWork.Reservations.Get(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _unitOfWork.Reservations.Remove(reservation);
            _unitOfWork.Complete();

            return Ok(reservation);
        }

        private bool ReservationExists(int id)
        {
            return _unitOfWork.Reservations.Get(id) != null;
        }
    }
}