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
            [RegularExpression(@"[0-9]+(\.[0-9] [0-9]?)?")]
            [Display(Name = "PricePerHour")]
            public double PricePerHour { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "IsAvailable")]
            public string IsAvailable { get; set; }
        }
    }
}