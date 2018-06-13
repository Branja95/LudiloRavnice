using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentApp.Models
{
    public class VehicleBindingModel
    {
        public class CreateVehicleBindingModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "VehicleType")]
            public string VehicleType { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Model")]
            public string Model { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Manufactor")]
            public string Manufactor { get; set; }
            
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "YearMade")]
            public DateTime YearMade { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Description")]
            public string Description { get; set; }

            [Required]
            [Display(Name = "PricePerHour")]
            public double PricePerHour { get; set; }

            [Required]
            [Display(Name = "Images")]
            public string Images { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Availability")]
            public string Availability { get; set; }
        }
    }
}