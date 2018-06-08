using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;

namespace RentApp.Controllers
{
    public class BranchOfficesController : ApiController
    {
        private RADBContext db;

        public BranchOfficesController(IUnitOfWork context)
        {
            db = context as RADBContext;
        }

        // GET: api/BranchOffices
        public IQueryable<BranchOffice> GetBranchOffices()
        {
            return db.BranchOffices;
        }

        // GET: api/BranchOffices/5
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult GetBranchOffice(int id)
        {
            BranchOffice branchOffice = db.BranchOffices.Find(id);
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

            db.Entry(branchOffice).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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
        public IHttpActionResult PostBranchOffice(BranchOffice branchOffice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BranchOffices.Add(branchOffice);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = branchOffice.Id }, branchOffice);
        }

        // DELETE: api/BranchOffices/5
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult DeleteBranchOffice(int id)
        {
            BranchOffice branchOffice = db.BranchOffices.Find(id);
            if (branchOffice == null)
            {
                return NotFound();
            }

            db.BranchOffices.Remove(branchOffice);
            db.SaveChanges();

            return Ok(branchOffice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchOfficeExists(int id)
        {
            return db.BranchOffices.Count(e => e.Id == id) > 0;
        }
    }
}