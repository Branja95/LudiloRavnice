using System.ComponentModel.DataAnnotations;

namespace RentVehicle.Models.Bindings
{
    public class VehicleTypeBindingModel
    {
        public class CreateVehicleTypeBindingModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TypeName")]
            public string TypeName { get; set; }
        }

        public class UpdateVehicleTypeBindingModel
        {
            [Required]
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TypeName")]
            public string TypeName { get; set; }
        }
    }
}
