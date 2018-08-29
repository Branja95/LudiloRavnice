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
using static RentApp.Models.VehicleTypeBindingModel;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/VehicleTypes")]
    public class VehicleTypesController : ApiController
    {
        private static object lockObjectForVehicleTypes = new object();

        private readonly IUnitOfWork unitOfWork;

        public VehicleTypesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/VehicleTypes/GetVehicleTypes
        [HttpGet]
        [Route("GetVehicleTypes")]
        [AllowAnonymous]
        public IEnumerable<VehicleType> GetVehicleTypes()
        {
            return unitOfWork.VehicleTypes.GetAll();
        }

        // GET: api/VehicleTypes/GetVehicleType/5
        [HttpGet]
        [Route("GetVehicleType")]
        [AllowAnonymous]
        public IHttpActionResult GetVehicleType([FromUri] long vehicleTypeId)
        {
            VehicleType vehicleType = unitOfWork.VehicleTypes.Get(vehicleTypeId);
            if (vehicleType == null)
            {
                return NotFound();
            }

            return Ok(vehicleType);
        }

        // PUT: api/VehicleTypes/PutVehicleType
        [HttpPut]
        [Route("PutVehicleType")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PutVehicleType(UpdateVehicleTypeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VehicleType vehicleType = unitOfWork.VehicleTypes.Get(model.Id);

            if (vehicleType == null)
            {
                return BadRequest("Vehicle type don't exists.");
            }

            vehicleType.TypeName = model.TypeName;

            try
            {
                lock (lockObjectForVehicleTypes)
                {
                    unitOfWork.VehicleTypes.Update(vehicleType);
                    unitOfWork.Complete();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleTypeExists(vehicleType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Vehicle type successfully updated.");
        }

        // POST: api/VehicleTypes/PostVehicleType
        [HttpPost]
        [Route("PostVehicleType")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostVehicleType(CreateVehicleTypeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VehicleType vehicleType = new VehicleType()
            {
                TypeName = model.TypeName
            };

            try
            {
                lock (lockObjectForVehicleTypes)
                {
                    unitOfWork.VehicleTypes.Add(vehicleType);
                    unitOfWork.Complete();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleTypeExists(vehicleType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Vehicle type successfully created.");
        }

        // DELETE: api/VehicleTypes/5
        [ResponseType(typeof(VehicleType))]
        public IHttpActionResult DeleteVehicleType(int id)
        {
            VehicleType vehicleType = unitOfWork.VehicleTypes.Get(id);
            if (vehicleType == null)
            {
                return NotFound();
            }

            try
            {
                lock (lockObjectForVehicleTypes)
                {
                    unitOfWork.VehicleTypes.Remove(vehicleType);
                    unitOfWork.Complete();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleTypeExists(vehicleType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(vehicleType);
        }

        private bool VehicleTypeExists(long id)
        {
            return unitOfWork.VehicleTypes.Get(id) != null;
        }
    }
}