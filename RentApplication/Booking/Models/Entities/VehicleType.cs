using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models.Entities
{
    public class VehicleType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string TypeName { get; set; }
    }
}
