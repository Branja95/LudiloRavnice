using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RentVehicle.Models.Bindings
{
    public class BranchOfficeBindingModel
    {
        public class CreateBranchOfficeBindingModel
        {
            [Display(Name = "ServiceId")]
            public int ServiceId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }

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

            [Required]
            [Display(Name = "Image")]
            public IFormFile Image { get; set; }
        }

        public class EditBranchOfficeBindingModel
        {
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Display(Name = "ServiceId")]
            public int ServiceId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)")]
            [Display(Name = "Latitude")]
            public double Latitude { get; set; }

            [Required]
            [RegularExpression(@"^[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$")]
            [Display(Name = "Longitude")]
            public double Longitude { get; set; }

            [Display(Name = "Image")]
            public IFormFile Image { get; set; }
        }
    }
}
