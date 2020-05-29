using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentVehicle.Models.Entities
{
    public class Vehicle
    {
        public long Id { get; set; }
        public string Model { get; set; }
        public string Manufactor { get; set; }
        public DateTime YearMade { get; set; }
        public string Description { get; set; }
        public double PricePerHour { get; set; }
        public string Images { get; set; }
        public bool IsAvailable { get; set; }
        public virtual VehicleType VehicleType { get; set; }
    }
}
