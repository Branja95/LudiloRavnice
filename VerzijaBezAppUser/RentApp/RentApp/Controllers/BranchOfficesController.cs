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
using System.Threading.Tasks;
using static RentApp.Models.BranchOfficeBindingModel;

namespace RentApp.Controllers
{
    public class BranchOfficesController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public BranchOfficesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/BranchOffices
        public IEnumerable<BranchOffice> GetBranchOffices()
        {
            return unitOfWork.BranchOffices.GetAll();
        }

        // GET: api/BranchOffices/5
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult GetBranchOffice(int id)
        {
            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(id);
            if (branchOffice == null)
            {
                return NotFound();
            }

            return Ok(branchOffice);
        }

        // PUT: api/BranchOffices/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBranchOffice(int id, BranchOffice branchOffice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branchOffice.Id)
            {
                return BadRequest();
            }

            try
            {
                unitOfWork.BranchOffices.Update(branchOffice);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchOfficeExists(id))
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
        
        // POST: api/BranchOffices
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult PostBranchOffice(CreateBranchOfficeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BranchOffice branchOffice = new BranchOffice
            {
                Image = model.Image,
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            unitOfWork.BranchOffices.Add(branchOffice);
            unitOfWork.Complete();
            
            return Ok("BranchOffice successfully created");
        }

        // DELETE: api/BranchOffices/5
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult DeleteBranchOffice(int id)
        {
            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(id);
            if (branchOffice == null)
            {
                return NotFound();
            }

            unitOfWork.BranchOffices.Remove(branchOffice);
            unitOfWork.Complete();

            return Ok(branchOffice);
        }

        private bool BranchOfficeExists(int id)
        {
            return unitOfWork.BranchOffices.Get(id) != null;
        }
    }
}