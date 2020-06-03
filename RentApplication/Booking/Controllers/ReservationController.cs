using System;
using System.Collections.Generic;
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

        private readonly string accountServiceUrl;
        private readonly string vehicleServiceUrl;
        private readonly string branchOfficeUrl;

        public ReservationController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> applicationUserManager,
            IConfiguration configuration)
        {
            _userManager = applicationUserManager;
            _unitOfWork = unitOfWork;

            accountServiceUrl = configuration["AccountService:AccountServiceUrl"];
            vehicleServiceUrl = configuration["RentVehicleService:VehicleServiceUrl"];
            branchOfficeUrl = configuration["RentVehicleService:BranchOfficeUrl"];
        }


        [HttpPost]
        [Route("PostReservation")]
        [Authorize(Roles = "Administrator, Manager, Client")]
        public async Task<IActionResult> PostReservationAsync([FromForm] CreateReservationBindingModel model)
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
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(accountServiceUrl + user.Id).ConfigureAwait(true);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        httpResponseMessage = await httpClient.GetAsync(vehicleServiceUrl + model.VehicleId).ConfigureAwait(true);
                        if(httpResponseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            Vehicle vehicle = await httpResponseMessage.Content.ReadAsAsync<Vehicle>().ConfigureAwait(false);
                            if(string.IsNullOrEmpty(VerifyVehicle(vehicle, model)))
                            {
                                return BadRequest(VerifyVehicle(vehicle, model));
                            }

                            httpResponseMessage = await httpClient.GetAsync(branchOfficeUrl + model.RentBranchOfficeId).ConfigureAwait(true);
                            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                            {
                                BranchOffice rentBranchOffice = await httpResponseMessage.Content.ReadAsAsync<BranchOffice>().ConfigureAwait(false);
                                if (rentBranchOffice == null)
                                {
                                    BadRequest("Non-existent rent branch office.");
                                }
                                else
                                {
                                    httpResponseMessage = await httpClient.GetAsync(branchOfficeUrl + model.ReturnBranchOfficeId).ConfigureAwait(true);
                                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                                    {
                                        BranchOffice returnBranchOffice = await httpResponseMessage.Content.ReadAsAsync<BranchOffice>().ConfigureAwait(false);
                                        if (returnBranchOffice == null)
                                        {
                                            BadRequest("Non-existent return rent branch office.");
                                        }
                                        else
                                        {
                                            lock (lockObjectForReservations)
                                            {
                                                IEnumerable<Reservation> vehicleReservations = _unitOfWork.Reservations.Find(r => r.Id == vehicle.Id);
                                                int numOfexcessiveReservations = vehicleReservations.Count(r => (r.ReservationStart <= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart <= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd));

                                                if (numOfexcessiveReservations > 0)
                                                {
                                                    BadRequest("The vehicle was rented in a given period.");
                                                }

                                                Reservation reservation = new Reservation()
                                                {
                                                    VehicleId = vehicle.Id,
                                                    UserId = user.Id,
                                                    ReservationStart = model.ReservationStart,
                                                    ReservationEnd = model.ReservationEnd,
                                                    RentBranchOfficeId = rentBranchOffice.Id,
                                                    ReturnBranchOfficeId = returnBranchOffice.Id
                                                };

                                                try
                                                {
                                                    _unitOfWork.Reservations.Add(reservation);
                                                    _unitOfWork.Complete();
                                                }
                                                catch (Exception)
                                                {
                                                    return NotFound();
                                                }

                                                return Ok();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return BadRequest();
                }
            }
        }

        private string VerifyVehicle(Vehicle vehicle, CreateReservationBindingModel reservation)
        {
            string errorMessage = string.Empty;

            if (vehicle == null)
            {
                errorMessage = "Vehicle doesn't exist.";
            }
            else
            {
                if (!vehicle.IsAvailable)
                {
                    errorMessage = "Vehicle isn't available.";
                }
                if (reservation.ReservationStart >= reservation.ReservationEnd)
                {
                    errorMessage = "Reservation start date and time must be greater than reservation end date and time.";
                    BadRequest();
                }
                if (reservation.ReservationStart < DateTime.Now)
                {
                    errorMessage = "Reservation start date and time must be greater than current date and time.";
                }
                if (reservation.ReservationEnd < DateTime.Now)
                {
                    errorMessage = "Reservation end date and time must be greater than current date and time.";
                }
            }

            return errorMessage;
        }
    }
}