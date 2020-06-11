using Microsoft.AspNetCore.Identity;
using RentVehicle.Models.Entities;
using RentVehicle.Models.IdentityUsers;
using RentVehicle.Persistance;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RentVehicle.DB
{
    public class EntryData
    {
        public static async Task Initialize(RentVehicleDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
        }
    }
}
