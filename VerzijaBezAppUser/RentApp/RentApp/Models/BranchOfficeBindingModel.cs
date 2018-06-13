using System.ComponentModel.DataAnnotations;

namespace RentApp.Models
{
    public class BranchOfficeBindingModel
    {
        public class CreateBranchOfficeBindingModel
        {

            [DataType(DataType.ImageUrl)]
            [Display(Name = "Image")]
            public string Image { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [RegularExpression(@"^\d+\.\d{0,2}$")]
            [Range(0, 9999999999999999.99)]
            [Display(Name = "Latitude")]
            public double Latitude { get; set; }

            [Required]
            [RegularExpression(@"^\d+\.\d{0,2}$")]
            [Range(0, 9999999999999999.99)]
            [Display(Name = "Longitude")]
            public double Longitude { get; set; }
        }
    }
}