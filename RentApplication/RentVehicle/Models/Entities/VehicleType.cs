using System.ComponentModel.DataAnnotations.Schema;

namespace RentVehicle.Models.Entities
{
    public class VehicleType
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
    }
}
