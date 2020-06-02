using System.ComponentModel.DataAnnotations;

namespace Booking.Models.Bindings
{
    public class RatingBindingModel
    {
        public class CreateRatingBindingModel
        {
            [Required]
            [Display(Name = "ServiceId")]
            public int ServiceId { get; set; }

            [Required]
            [Display(Name = "Value")]
            public int Value { get; set; }
        }

        public class EditRatingBindingModel
        {
            [Required]
            [Display(Name = "Id")]
            public int Id { get; set; }

            [Required]
            [Display(Name = "Value")]
            public int Value { get; set; }
        }

        public class ClientRating
        {
            public string User { get; set; }

            public int Value { get; set; }
        }
    }
}
