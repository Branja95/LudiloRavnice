using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models.Entities
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
    }
}
