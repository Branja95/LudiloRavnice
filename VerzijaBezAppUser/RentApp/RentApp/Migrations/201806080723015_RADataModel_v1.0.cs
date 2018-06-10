namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RADataModel_v10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BranchOffices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.String(),
                        Address = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Service_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .Index(t => t.Service_Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        Text = c.String(),
                        Service_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .Index(t => t.Service_Id);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Value = c.Int(nullable: false),
                        Service_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .Index(t => t.Service_Id);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReservationStart = c.DateTime(nullable: false),
                        ReservationEnd = c.DateTime(nullable: false),
                        RentBranchOffice_Id = c.Int(),
                        ReturnBranchOffice_Id = c.Int(),
                        Vehicle_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BranchOffices", t => t.RentBranchOffice_Id)
                .ForeignKey("dbo.BranchOffices", t => t.ReturnBranchOffice_Id)
                .ForeignKey("dbo.Vehicles", t => t.Vehicle_Id)
                .Index(t => t.RentBranchOffice_Id)
                .Index(t => t.ReturnBranchOffice_Id)
                .Index(t => t.Vehicle_Id);
            
            CreateTable(
                "dbo.Vehicles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Model = c.String(),
                        Manufactor = c.String(),
                        YearMade = c.DateTime(nullable: false),
                        Description = c.String(),
                        PricePerHour = c.Double(nullable: false),
                        IsAvailable = c.Boolean(nullable: false),
                        VehicleType_Id = c.Int(),
                        Service_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleTypes", t => t.VehicleType_Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .Index(t => t.VehicleType_Id)
                .Index(t => t.Service_Id);
            
            CreateTable(
                "dbo.VehicleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AppUsers", "FirstName", c => c.String());
            AddColumn("dbo.AppUsers", "LastName", c => c.String());
            AddColumn("dbo.AppUsers", "Image", c => c.String());
            AddColumn("dbo.AppUsers", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.AppUsers", "IsLogged", c => c.Boolean(nullable: false));
            AddColumn("dbo.AppUsers", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("dbo.Services", "OwnerId", c => c.Int(nullable: false));
            AddColumn("dbo.Services", "LogoImage", c => c.String());
            AddColumn("dbo.Services", "EmailAddress", c => c.String());
            AddColumn("dbo.Services", "Description", c => c.String());
            AddColumn("dbo.Services", "IsApproved", c => c.Boolean(nullable: false));
            DropColumn("dbo.AppUsers", "FullName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AppUsers", "FullName", c => c.String());
            DropForeignKey("dbo.Vehicles", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Ratings", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Comments", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.BranchOffices", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Reservations", "Vehicle_Id", "dbo.Vehicles");
            DropForeignKey("dbo.Vehicles", "VehicleType_Id", "dbo.VehicleTypes");
            DropForeignKey("dbo.Reservations", "ReturnBranchOffice_Id", "dbo.BranchOffices");
            DropForeignKey("dbo.Reservations", "RentBranchOffice_Id", "dbo.BranchOffices");
            DropIndex("dbo.Vehicles", new[] { "Service_Id" });
            DropIndex("dbo.Vehicles", new[] { "VehicleType_Id" });
            DropIndex("dbo.Reservations", new[] { "Vehicle_Id" });
            DropIndex("dbo.Reservations", new[] { "ReturnBranchOffice_Id" });
            DropIndex("dbo.Reservations", new[] { "RentBranchOffice_Id" });
            DropIndex("dbo.Ratings", new[] { "Service_Id" });
            DropIndex("dbo.Comments", new[] { "Service_Id" });
            DropIndex("dbo.BranchOffices", new[] { "Service_Id" });
            DropColumn("dbo.Services", "IsApproved");
            DropColumn("dbo.Services", "Description");
            DropColumn("dbo.Services", "EmailAddress");
            DropColumn("dbo.Services", "LogoImage");
            DropColumn("dbo.Services", "OwnerId");
            DropColumn("dbo.AppUsers", "IsApproved");
            DropColumn("dbo.AppUsers", "IsLogged");
            DropColumn("dbo.AppUsers", "DateOfBirth");
            DropColumn("dbo.AppUsers", "Image");
            DropColumn("dbo.AppUsers", "LastName");
            DropColumn("dbo.AppUsers", "FirstName");
            DropTable("dbo.VehicleTypes");
            DropTable("dbo.Vehicles");
            DropTable("dbo.Reservations");
            DropTable("dbo.Ratings");
            DropTable("dbo.Comments");
            DropTable("dbo.BranchOffices");
        }
    }
}
