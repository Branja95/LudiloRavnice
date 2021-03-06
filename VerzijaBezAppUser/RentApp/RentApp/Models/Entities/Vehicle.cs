﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Vehicle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Model { get; set; }

        public string Manufactor { get; set; }

        public virtual VehicleType VehicleType { get; set; }

        public DateTime YearMade { get; set; }

        public string Description { get; set; }

        public double PricePerHour { get; set; }

        public string Images { get; set; }

        public bool IsAvailable { get; set; }

    }
}