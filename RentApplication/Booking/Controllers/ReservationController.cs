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

        private readonly string _findUserEndpoint;
        private readonly IUnitOfWork _unitOfWork;


        private readonly string _isAccountApprovedEndpoint;        private readonly string _vehicleServiceEndpoint;
        private readonly string _vehicleServiceReserveEndpoint;
        private readonly string _branchOfficeEndpoint;

        public ReservationController(IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _isAccountApprovedEndpoint = configuration["AccountService:IsAccountApprovedEndpoint"];
            _findUserEndpoint = configuration["AccountService:FindUserEndpoint"];
            _vehicleServiceEndpoint = configuration["RentVehicleService:GetVehicleEndpoint"];
            _vehicleServiceReserveEndpoint = configuration["RentVehicleService:GetReserveVehicleEndpoint"];
            _branchOfficeEndpoint = configuration["RentVehicleService:GetBranchOfficeEndpoint"];
        }

        [HttpGet]
        [Route("GetReservations")]
        public IActionResult GetReservations()
        {
            return Ok(_unitOfWork.Reservations.GetAll());
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
                ApplicationUser user = await FindUser();
                if(user == null)
                {
                    return BadRequest();
                }
                else
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_isAccountApprovedEndpoint + user.Id).ConfigureAwait(true);
                        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            httpResponseMessage = await httpClient.GetAsync(_vehicleServiceEndpoint + model.VehicleId).ConfigureAwait(true);
                            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                            {
                                Vehicle vehicle = await httpResponseMessage.Content.ReadAsAsync<Vehicle>().ConfigureAwait(false);
                                if (!string.IsNullOrEmpty(VerifyVehicle(vehicle, model)))
                                {
                                    return BadRequest(VerifyVehicle(vehicle, model));
                                }

                                httpResponseMessage = await httpClient.GetAsync(_branchOfficeEndpoint + model.RentBranchOfficeId).ConfigureAwait(true);
                                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                                {
                                    BranchOffice rentBranchOffice = await httpResponseMessage.Content.ReadAsAsync<BranchOffice>().ConfigureAwait(false);
                                    if (rentBranchOffice == null)
                                    {
                                        BadRequest("Non-existent rent branch office.");
                                    }
                                    else
                                    {
                                        httpResponseMessage = await httpClient.GetAsync(_branchOfficeEndpoint + model.ReturnBranchOfficeId).ConfigureAwait(true);
                                        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                                        {
                                            BranchOffice returnBranchOffice = await httpResponseMessage.Content.ReadAsAsync<BranchOffice>().ConfigureAwait(false);
                                            if (returnBranchOffice == null)
                                            {
                                                BadRequest("Non-existent return rent branch office.");
                                            }
                                            else
                                            {
                                                HttpContent httpContent = new StringContent(vehicle.Id.ToString());
                                                httpResponseMessage = await httpClient.PutAsync(_vehicleServiceReserveEndpoint + vehicle.Id, httpContent).ConfigureAwait(true);
                                                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                                                {
                                                    lock (lockObjectForReservations)
                                                    {
                                                        IEnumerable<Reservation> vehicleReservations = _unitOfWork.Reservations.Find(r => r.Id == vehicle.Id);
                                                        int numOfexcessiveReservations = vehicleReservations.Count(r => (r.ReservationStart <= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart <= model.ReservationStart && r.ReservationEnd <= model.ReservationEnd) || (r.ReservationStart >= model.ReservationStart && r.ReservationEnd >= model.ReservationEnd));

                                                        if (numOfexcessiveReservations > 0)
                                                        {
                                                            return BadRequest("The vehicle was rented in a given period.");
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
                        }
                        return BadRequest();
                    }
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
                /*
                if (reservation.ReservationStart < DateTime.Now)
                {
                    errorMessage = "Reservation start date and time must be greater than current date and time.";
                }
                if (reservation.ReservationEnd < DateTime.Now)
                {
                    errorMessage = "Reservation end date and time must be greater than current date and time.";
                }
                */
            }

            return errorMessage;
        }
    }
}