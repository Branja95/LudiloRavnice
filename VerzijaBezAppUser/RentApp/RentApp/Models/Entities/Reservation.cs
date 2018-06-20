using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentApp.Models.Entities
{
    public class Reservation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string UserId { get; set; }

        public DateTime ReservationStart { get; set; }

        public DateTime ReservationEnd { get; set; }

        public virtual Vehicle Vehicle { get; set; }

        public virtual BranchOffice RentBranchOffice { get; set; }

        public virtual BranchOffice ReturnBranchOffice { get; set; }
    }
}