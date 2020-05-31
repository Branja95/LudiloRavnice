using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentVehicle.Models.Bindings
{
    public class VehicleBindingModel
    {
        public class CreateVehicleBindingModel
        {
            [Required]
            [Display(Name = "ServiceId")]
            public long ServiceId { get; set; }

            [Required]
            [Display(Name = "VehicleTypeId")]
            public long VehicleTypeId { get; set; }

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
            [RegularExpression(@"^[+]?(\d*\.)?\d+$")]
            [Display(Name = "PricePerHour")]
            public double PricePerHour { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "IsAvailable")]
            public string IsAvailable { get; set; }

            [Required]
            [Display(Name = "Images")]
            public IEnumerable<IFormFile> Images { get; set; }
        }

        public class VehicleIdBindingModel
        {
            [Required]
            [Display(Name = "VehicleId")]
            public long VehicleId { get; set; }
        }

        public class EditVehicleBindingModel
        {
            [Required]
            [Display(Name = "Id")]
            public long Id { get; set; }

            [Required]
            [Display(Name = "VehicleTypeId")]
            public long VehicleTypeId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Model")]
            public string Model { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Manufactor")]
            public string Manufactor { get; set; }

            [Required]
            [Display(Name = "YearMade")]
            public DateTime YearMade { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Description")]
            public string Description { get; set; }

            [Required]
            [RegularExpression(@"^[+]?(\d*\.)?\d+$")]
            [Display(Name = "PricePerHour")]
            public double PricePerHour { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "IsAvailable")]
            public string IsAvailable { get; set; }

            [Required]
            [Display(Name = "Images")]
            public IEnumerable<IFormFile> Images { get; set; }
        }
    }
}
