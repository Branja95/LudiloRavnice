using System.ComponentModel.DataAnnotations.Schema;

namespace RentVehicle.Models.Entities
{
    public class ServiceForApproval
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public Service Service { get; set; }
    }
}
