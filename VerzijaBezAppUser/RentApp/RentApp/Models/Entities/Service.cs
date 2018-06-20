using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentApp.Models.Entities
{
    public class Service
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Creator { get; set; }

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