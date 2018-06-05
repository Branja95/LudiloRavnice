using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string LogoImage { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public virtual List<BranchOffice> BranchOfficces { get; set; }
        public virtual List<Vehicle> Vehicles { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Rating> Ratings { get; set; }
        public bool IsApproved { get; set; }


    }
}