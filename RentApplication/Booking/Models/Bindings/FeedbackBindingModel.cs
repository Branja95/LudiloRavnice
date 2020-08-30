using Booking.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Booking.Models.Bindings
{
    public class FeedbackBindingModel
    {
        public class CreateUserFeedbackBindingModel
        {
            [Required]
            [Display(Name = "ServiceId")]
            public int ServiceId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Text")]
            public string Text { get; set; }

            [Required]
            [Display(Name = "Value")]
            public int Value { get; set; }

        }

        public class UserFeedbackBindingModel
        {
            public long Id { get; set; }
            public string UserId { get; set; }
            public long ServiceId { get; set; }
            public string UserFirstName { get; set; }
            public string UserLastName { get; set; }
            public string Text { get; set; }
            public int Value { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}
