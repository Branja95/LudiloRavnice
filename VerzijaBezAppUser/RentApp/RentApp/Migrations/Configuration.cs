namespace RentApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using RentApp.Models.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<RentApp.Persistance.RADBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RentApp.Persistance.RADBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            //context.People.AddOrUpdate(
            //  p => p.FullName,
            //  new Person { FullName = "Andrew Peters" },
            //  new Person { FullName = "Brice Lambson" },
            //  new Person { FullName = "Rowan Miller" }
            //);

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Manager"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Manager" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "AppUser"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "AppUser" };

                manager.Create(role);
            }

            var userStore = new UserStore<RAIdentityUser>(context);
            var userManager = new UserManager<RAIdentityUser>(userStore);

            if (!context.Users.Any(u => u.UserName == "branici@gmail.com"))
            {
                var user = new RAIdentityUser() { UserName = "branici@gmail.com", Email = "branici@gmail.com", EmailConfirmed = true, PasswordHash = RAIdentityUser.HashPassword("Branja95"), FirstName = "Branko", LastName = "Jelic", Image = "", DateOfBirth = DateTime.Parse("07.09.1995") };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "david.djekic@gmail.com"))
            {
                var user = new RAIdentityUser() { UserName = "david.djekic@gmail.com", Email = "david.djekic@gmail.com", EmailConfirmed = true, PasswordHash = RAIdentityUser.HashPassword("David95"), FirstName = "David", LastName = "Djekic", Image = "", DateOfBirth = DateTime.Parse("02.06.1995") };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");

            }

            context.SaveChanges();

            Service servicePS = new Service() { Id = 1, Creator = context.Users.SingleOrDefault(u => u.UserName == "david.djekic@gmail.com").Id, Name = "Posavinski Rent a Car", LogoImage = "", EmailAddress = "posavina.rent@gmail.com", Description = "Prvi Posavinski Renta a car!", BranchOfficces = new System.Collections.Generic.List<BranchOffice>(), Vehicles = new System.Collections.Generic.List<Vehicle>(), Comments = new System.Collections.Generic.List<Comment>(), Ratings = new System.Collections.Generic.List<Rating>(), IsApproved = true };
            Service serviceSR = new Service() { Id = 2, Creator = context.Users.SingleOrDefault(u => u.UserName == "branici@gmail.com").Id, Name = "Sremski Rent a Car", LogoImage = "", EmailAddress = "srem.rent@gmail.com", Description = "Prvi Sremski Renta a car!", BranchOfficces = new System.Collections.Generic.List<BranchOffice>(), Vehicles = new System.Collections.Generic.List<Vehicle>(), Comments = new System.Collections.Generic.List<Comment>(), Ratings = new System.Collections.Generic.List<Rating>(), IsApproved = true };

            BranchOffice branchOfficeGR = new BranchOffice() { Id = 1, Address = "Partizanska 2", Image = "", Latitude = 0, Longitude = 0 };
            BranchOffice branchOfficeSB = new BranchOffice() { Id = 2, Address = "Heroja Davida 3", Image = "", Latitude = 0, Longitude = 0 };

            VehicleType vehicleTypeL = new VehicleType() { Id = 1, TypeName = "Limuzina" };
            VehicleType vehicleTypeH = new VehicleType() { Id = 2, TypeName = "Hecbek" };

            Vehicle vehicleG = new Vehicle() { Id = 1, Model = "Golf 2", Manufactor = "WV", VehicleType = vehicleTypeL, YearMade = DateTime.Parse("01.02.1990"), Description = "Vrhunsko auto!", PricePerHour = 1.2, Images = new System.Collections.Generic.List<string>(), IsAvailable = true };
            Vehicle vehicleZ = new Vehicle() { Id = 2, Model = "750", Manufactor = "Zastava", VehicleType = vehicleTypeH, YearMade = DateTime.Parse("01.02.1975"), Description = "Nacionalna klasa!", PricePerHour = 2.1, Images = new System.Collections.Generic.List<string>(), IsAvailable = true };

            Comment comment1 = new Comment() { Id = 1, UserId = context.Users.SingleOrDefault(u => u.UserName == "branici@gmail.com").Id, DateTime = DateTime.Now, Text = "Poz iz Grabova." };
            Comment comment2 = new Comment() { Id = 2, UserId = context.Users.SingleOrDefault(u => u.UserName == "david.djekic@gmail.com").Id, DateTime = DateTime.Now, Text = "Poz iz Srpskog Broda." };

            Rating rating1 = new Rating() { Id = 1, UserId = context.Users.SingleOrDefault(u => u.UserName == "branici@gmail.com").Id, Value = 5 };
            Rating rating2 = new Rating() { Id = 2, UserId = context.Users.SingleOrDefault(u => u.UserName == "david.djekic@gmail.com").Id, Value = 5 };

            servicePS.BranchOfficces.Add(branchOfficeSB);
            servicePS.Vehicles.Add(vehicleG);
            servicePS.Comments.Add(comment1);
            servicePS.Ratings.Add(rating1);

            serviceSR.BranchOfficces.Add(branchOfficeGR);
            serviceSR.Vehicles.Add(vehicleZ);
            serviceSR.Comments.Add(comment2);
            serviceSR.Ratings.Add(rating2);
            
            context.BranchOffices.AddOrUpdate(branchOfficeGR);
            context.BranchOffices.AddOrUpdate(branchOfficeSB);

            context.VehicleTypes.AddOrUpdate(vehicleTypeH);
            context.VehicleTypes.AddOrUpdate(vehicleTypeL);

            context.Vehicles.AddOrUpdate(vehicleG);
            context.Vehicles.AddOrUpdate(vehicleZ);

            context.Comments.AddOrUpdate(comment1);
            context.Comments.AddOrUpdate(comment2);

            context.Ratings.AddOrUpdate(rating1);
            context.Ratings.AddOrUpdate(rating2);
            
            context.Services.AddOrUpdate(servicePS);
            context.Services.AddOrUpdate(serviceSR);

            context.SaveChanges();
        }
    }
}
