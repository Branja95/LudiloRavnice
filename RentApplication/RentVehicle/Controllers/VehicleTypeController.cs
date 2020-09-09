using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentVehicle.Models.Entities;
using RentVehicle.Persistance.UnitOfWork;
using static RentVehicle.Models.Bindings.VehicleTypeBindingModel;

namespace RentVehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleTypeController : ControllerBase
    {
        private static object lockObjectForVehicleTypes = new object();

        private readonly IUnitOfWork _unitOfWork;

        public VehicleTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetVehicleTypes")]
        [AllowAnonymous]
        public IEnumerable<VehicleType> GetVehicleTypes()
        {
            return _unitOfWork.VehicleTypes.GetAll();
        }


        [HttpGet]
        [Route("GetVehicleType")]
        [AllowAnonymous]
        public IActionResult GetVehicleType(long vehicleTypeId)
        {
            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(vehicleTypeId);
            if (vehicleType == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(vehicleType);
            }
        }


        [HttpPost]
        [Route("PostVehicleType")]
        [Authorize(Roles = "Administrator")]
        public IActionResult PostVehicleType(CreateVehicleTypeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                VehicleType vehicleType = new VehicleType()
                {
                    TypeName = model.TypeName
                };

                try
                {
                    lock (lockObjectForVehicleTypes)
                    {
                        _unitOfWork.VehicleTypes.Add(vehicleType);
                        _unitOfWork.Complete();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_unitOfWork.VehicleTypes.Get(vehicleType.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(_unitOfWork.VehicleTypes.GetAll());
            }
        }


        [HttpPut]
        [Route("PutVehicleType")]
        [Authorize(Roles = "Administrator")]
        public IActionResult PutVehicleType(UpdateVehicleTypeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(model.Id);
                if (vehicleType == null)
                {
                    return BadRequest();
                }
                else
                {
                    vehicleType.TypeName = model.TypeName;
                    try
                    {
                        lock (lockObjectForVehicleTypes)
                        {
                            _unitOfWork.VehicleTypes.Update(vehicleType);
                            _unitOfWork.Complete();
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (_unitOfWork.VehicleTypes.Get(vehicleType.Id) == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok();
                }
            }
        }


        [HttpDelete]
        [Route("DeleteVehicleType")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteVehicleType(int id)
        {
            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(id);
            if (vehicleType == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    lock (lockObjectForVehicleTypes)
                    {
                        _unitOfWork.VehicleTypes.Remove(vehicleType);
                        _unitOfWork.Complete();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_unitOfWork.VehicleTypes.Get(vehicleType.Id) == null)
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
        }
    }
}