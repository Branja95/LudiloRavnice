﻿using System.Collections.Generic;
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
        public IActionResult GetVehicleType([FromForm] long vehicleTypeId)
        {
            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(vehicleTypeId);
            if (vehicleType == null)
            {
                return NotFound();
            }

            return Ok(vehicleType);
        }

        [HttpPut]
        [Route("PutVehicleType")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutVehicleType(UpdateVehicleTypeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(model.Id);

            if (vehicleType == null)
            {
                return BadRequest("Vehicle type don't exists.");
            }

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

        [HttpPost]
        [Route("PostVehicleType")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostVehicleType(CreateVehicleTypeBindingModel model)
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
                    _unitOfWork.VehicleTypes.Add(vehicleType);
                    _unitOfWork.Complete();
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

        public IActionResult DeleteVehicleType(int id)
        {
            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(id);
            if (vehicleType == null)
            {
                return NotFound();
            }

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
            return _unitOfWork.VehicleTypes.Get(id) != null;
        }
    }
}