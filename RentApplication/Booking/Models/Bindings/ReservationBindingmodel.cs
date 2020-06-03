using System;
using System.ComponentModel.DataAnnotations;

namespace Booking.Models.Bindings
{
    public class ReservationBindingmodel
    {
        public class CreateReservationBindingModel
        {
            [Required]
            [Display(Name = "VehicleId")]
            public long VehicleId { get; set; }

            [Required]
            [Display(Name = "RentBranchOfficeId")]
            public long RentBranchOfficeId { get; set; }

            [Required]
            [Display(Name = "ReturnBranchOfficeId")]
            public long ReturnBranchOfficeId { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "ReservationStart")]
            public DateTime ReservationStart { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "ReservationEnd")]
            public DateTime ReservationEnd { get; set; }

        }
    }
}
