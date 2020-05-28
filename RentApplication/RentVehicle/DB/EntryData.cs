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
            try
            {
                var a = context.Users.SingleOrDefault(u => u.UserName == "thelood").Id;
            }
            catch (Exception e)
            {

            }

            Service servicePS = new Service() { Creator = context.Users.SingleOrDefault(u => u.UserName == "thelood").Id, Name = "Posavinski Rent a Car", LogoImage = "", EmailAddress = "posavina.rent@gmail.com", Description = "Prvi Posavinski Renta a car!", BranchOfficces = new System.Collections.Generic.List<BranchOffice>(), Vehicles = new System.Collections.Generic.List<Vehicle>(), Comments = new System.Collections.Generic.List<Comment>(), Ratings = new System.Collections.Generic.List<Rating>(), IsApproved = true };
            Service serviceSR = new Service() { Creator = context.Users.SingleOrDefault(u => u.UserName == "branja").Id, Name = "Sremski Rent a Car", LogoImage = "", EmailAddress = "srem.rent@gmail.com", Description = "Prvi Sremski Renta a car!", BranchOfficces = new System.Collections.Generic.List<BranchOffice>(), Vehicles = new System.Collections.Generic.List<Vehicle>(), Comments = new System.Collections.Generic.List<Comment>(), Ratings = new System.Collections.Generic.List<Rating>(), IsApproved = true };

            BranchOffice branchOfficeGR = new BranchOffice() { Address = "Partizanska 2", Image = @"C:\Users\Exlrt-0115\Desktop\test1.jpg", Latitude = 0, Longitude = 0 };
            BranchOffice branchOfficeSB = new BranchOffice() { Address = "Heroja Davida 3", Image = @"C:\Users\Exlrt-0115\Desktop\test2.jpg", Latitude = 0, Longitude = 0 };

            VehicleType vehicleTypeL = new VehicleType() { TypeName = "Limuzina" };
            VehicleType vehicleTypeH = new VehicleType() { TypeName = "Hecbek" };

            Vehicle vehicleG = new Vehicle() { Model = "Golf 2", Manufactor = "WV", VehicleType = vehicleTypeL, YearMade = DateTime.Parse("01.02.1990"), Description = "Vrhunsko auto!", PricePerHour = 1.2, Images = "", IsAvailable = true };
            Vehicle vehicleZ = new Vehicle() { Model = "750", Manufactor = "Zastava", VehicleType = vehicleTypeH, YearMade = DateTime.Parse("01.02.1975"), Description = "Nacionalna klasa!", PricePerHour = 2.1, Images = "", IsAvailable = true };

            Comment comment1 = new Comment() { UserId = context.Users.SingleOrDefault(u => u.UserName == "branja").Id, DateTime = DateTime.Now, Text = "Poz iz Grabova." };
            Comment comment2 = new Comment() { UserId = context.Users.SingleOrDefault(u => u.UserName == "thelood").Id, DateTime = DateTime.Now, Text = "Poz iz Srpskog Broda." };

            Rating rating1 = new Rating() { UserId = context.Users.SingleOrDefault(u => u.UserName == "branja").Id, Value = 5 };
            Rating rating2 = new Rating() { UserId = context.Users.SingleOrDefault(u => u.UserName == "thelood").Id, Value = 5 };

            servicePS.BranchOfficces.Add(branchOfficeSB);
            servicePS.Vehicles.Add(vehicleG);
            servicePS.Comments.Add(comment1);
            servicePS.Ratings.Add(rating1);

            serviceSR.BranchOfficces.Add(branchOfficeGR);
            serviceSR.Vehicles.Add(vehicleZ);
            serviceSR.Comments.Add(comment2);
            serviceSR.Ratings.Add(rating2);

            context.BranchOffices.Add(branchOfficeGR);
            context.BranchOffices.Add(branchOfficeSB);

            context.VehicleTypes.Add(vehicleTypeH);
            context.VehicleTypes.Add(vehicleTypeL);

            context.Vehicles.Add(vehicleG);
            context.Vehicles.Add(vehicleZ);

            context.Services.Add(servicePS);
            context.Services.Add(serviceSR);
            /*
            context.Comments.AddOrUpdate(comment1);
            context.Comments.AddOrUpdate(comment2);

            context.Ratings.AddOrUpdate(rating1);
            context.Ratings.AddOrUpdate(rating2);

            */

            context.Database.EnsureCreated();
        }
    }
}
