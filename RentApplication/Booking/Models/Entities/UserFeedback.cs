﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models.Entities
{
    public class UserFeedback
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ServiceId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public int Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}