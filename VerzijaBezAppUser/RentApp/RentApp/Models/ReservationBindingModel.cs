using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models
{
    public class ReservationBindingModel
    {
        public class CreateReservationBindingModel
        {
            [Required]
            [Display(Name = "VehicleId")]
            public long VehicleId { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "ReservationStart")]
            public DateTime ReservationStart { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "ReservationEnd")]
            public DateTime ReservationEnd { get; set; }

            [Required]
            [Display(Name = "RentBranchOfficeId")]
            public long RentBranchOfficeId { get; set; }

            [Required]
            [Display(Name = "ReturnBranchOfficeId")]
            public long ReturnBranchOfficeId { get; set; }

        }
    }
}