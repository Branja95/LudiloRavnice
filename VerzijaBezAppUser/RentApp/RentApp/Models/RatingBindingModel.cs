using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models
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