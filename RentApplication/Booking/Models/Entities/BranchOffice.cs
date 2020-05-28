using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models.Entities
{
    public class BranchOffice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
