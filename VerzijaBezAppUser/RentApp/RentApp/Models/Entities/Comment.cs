using System;

namespace RentApp.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime DateTime { get; set; }

        public string Text { get; set; }

    }
}