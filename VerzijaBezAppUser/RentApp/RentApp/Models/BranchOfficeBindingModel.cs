﻿using System.ComponentModel.DataAnnotations;

namespace RentApp.Models
{
    public class BranchOfficeBindingModel
    {
        public class CreateBranchOfficeBindingModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)")]
            [Display(Name = "Latitude")]
            public double Latitude { get; set; }

            [Required]
            [RegularExpression(@"^[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$")]
            [Display(Name = "Longitude")]
            public double Longitude { get; set; }

        }
    }
}