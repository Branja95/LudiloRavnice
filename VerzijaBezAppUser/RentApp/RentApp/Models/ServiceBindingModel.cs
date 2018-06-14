using System.ComponentModel.DataAnnotations;

namespace RentApp.Models
{
    public class ServiceBindingModel
    {
        public class CreateRentVehicleServiceBindingModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "ContactEmail")]
            public string ContactEmail { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Description")]
            public string Description { get; set; }

            [Required]
            [DataType(DataType.ImageUrl)]
            [Display(Name = "LogoImage")]
            public string LogoImage { get; set; }
        }
    }
}