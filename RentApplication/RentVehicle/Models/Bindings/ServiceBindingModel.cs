using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RentVehicle.Models.Bindings
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
            [DataType(DataType.Text)]
            [Display(Name = "Image")]
            public IFormFile Image { get; set; }
        }

        public class EditRentVehicleServiceBindingModel
        {
            [Required]
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Required]
            [Display(Name = "ServiceId")]
            public int ServiceId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "EmailAddress")]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Description")]
            public string Description { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Image")]
            public IFormFile Image { get; set; }
        }
    }
}
