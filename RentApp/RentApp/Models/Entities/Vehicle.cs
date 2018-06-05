using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Manufactor { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public DateTime YearMade { get; set; }
        public string Description { get; set; }
        public double PricePerHour { get; set; }
        public List<string> Images { get; set; }
        public bool IsAvailable { get; set; }

    }
}