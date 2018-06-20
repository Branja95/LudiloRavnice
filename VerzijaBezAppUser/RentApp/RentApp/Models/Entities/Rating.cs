using System.ComponentModel.DataAnnotations.Schema;

namespace RentApp.Models.Entities
{
    public class Rating
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string UserId { get; set; }

        public int Value { get; set; }
    }
}