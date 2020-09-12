using System;

namespace RentVehicle.Models
{
    public class Reservation
    {
        public long Id { get; set; }

        public string UserId { get; set; }

        public DateTime ReservationStart { get; set; }

        public DateTime ReservationEnd { get; set; }

        public long VehicleId { get; set; }

        public long RentBranchOfficeId { get; set; }

        public long ReturnBranchOfficeId { get; set; }
    }
}
