namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RADataModel_v21 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "RentBranchOffice_Id", "dbo.BranchOffices");
            DropForeignKey("dbo.Reservations", "ReturnBranchOffice_Id", "dbo.BranchOffices");
            DropForeignKey("dbo.Reservations", "Vehicle_Id", "dbo.Vehicles");
            DropForeignKey("dbo.Vehicles", "VehicleType_Id", "dbo.VehicleTypes");
            DropForeignKey("dbo.BranchOffices", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Comments", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Ratings", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Vehicles", "Service_Id", "dbo.Services");
            DropIndex("dbo.BranchOffices", new[] { "Service_Id" });
            DropIndex("dbo.Comments", new[] { "Service_Id" });
            DropIndex("dbo.Ratings", new[] { "Service_Id" });
            DropIndex("dbo.Reservations", new[] { "RentBranchOffice_Id" });
            DropIndex("dbo.Reservations", new[] { "ReturnBranchOffice_Id" });
            DropIndex("dbo.Reservations", new[] { "Vehicle_Id" });
            DropIndex("dbo.Vehicles", new[] { "VehicleType_Id" });
            DropIndex("dbo.Vehicles", new[] { "Service_Id" });
            DropPrimaryKey("dbo.BranchOffices");
            DropPrimaryKey("dbo.Comments");
            DropPrimaryKey("dbo.Ratings");
            DropPrimaryKey("dbo.Reservations");
            DropPrimaryKey("dbo.Vehicles");
            DropPrimaryKey("dbo.VehicleTypes");
            DropPrimaryKey("dbo.Services");
            CreateTable(
                "dbo.AccountForApproves",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ServiceForApproves",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Service_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.Service_Id)
                .Index(t => t.Service_Id);
            
            AddColumn("dbo.AspNetUsers", "DocumentImage", c => c.String());
            AddColumn("dbo.AspNetUsers", "IsApproved", c => c.Boolean(nullable: false));
            AlterColumn("dbo.BranchOffices", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.BranchOffices", "Service_Id", c => c.Long());
            AlterColumn("dbo.Comments", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Comments", "Service_Id", c => c.Long());
            AlterColumn("dbo.Ratings", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Ratings", "Service_Id", c => c.Long());
            AlterColumn("dbo.Reservations", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Reservations", "RentBranchOffice_Id", c => c.Long());
            AlterColumn("dbo.Reservations", "ReturnBranchOffice_Id", c => c.Long());
            AlterColumn("dbo.Reservations", "Vehicle_Id", c => c.Long());
            AlterColumn("dbo.Vehicles", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Vehicles", "VehicleType_Id", c => c.Long());
            AlterColumn("dbo.Vehicles", "Service_Id", c => c.Long());
            AlterColumn("dbo.VehicleTypes", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Services", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.BranchOffices", "Id");
            AddPrimaryKey("dbo.Comments", "Id");
            AddPrimaryKey("dbo.Ratings", "Id");
            AddPrimaryKey("dbo.Reservations", "Id");
            AddPrimaryKey("dbo.Vehicles", "Id");
            AddPrimaryKey("dbo.VehicleTypes", "Id");
            AddPrimaryKey("dbo.Services", "Id");
            CreateIndex("dbo.BranchOffices", "Service_Id");
            CreateIndex("dbo.Comments", "Service_Id");
            CreateIndex("dbo.Ratings", "Service_Id");
            CreateIndex("dbo.Reservations", "RentBranchOffice_Id");
            CreateIndex("dbo.Reservations", "ReturnBranchOffice_Id");
            CreateIndex("dbo.Reservations", "Vehicle_Id");
            CreateIndex("dbo.Vehicles", "VehicleType_Id");
            CreateIndex("dbo.Vehicles", "Service_Id");
            AddForeignKey("dbo.Reservations", "RentBranchOffice_Id", "dbo.BranchOffices", "Id");
            AddForeignKey("dbo.Reservations", "ReturnBranchOffice_Id", "dbo.BranchOffices", "Id");
            AddForeignKey("dbo.Reservations", "Vehicle_Id", "dbo.Vehicles", "Id");
            AddForeignKey("dbo.Vehicles", "VehicleType_Id", "dbo.VehicleTypes", "Id");
            AddForeignKey("dbo.BranchOffices", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Comments", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Ratings", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Vehicles", "Service_Id", "dbo.Services", "Id");
            DropColumn("dbo.AspNetUsers", "Image");
            DropColumn("dbo.AspNetUsers", "IsLogged");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsLogged", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Image", c => c.String());
            DropForeignKey("dbo.Vehicles", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Ratings", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Comments", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.BranchOffices", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Vehicles", "VehicleType_Id", "dbo.VehicleTypes");
            DropForeignKey("dbo.Reservations", "Vehicle_Id", "dbo.Vehicles");
            DropForeignKey("dbo.Reservations", "ReturnBranchOffice_Id", "dbo.BranchOffices");
            DropForeignKey("dbo.Reservations", "RentBranchOffice_Id", "dbo.BranchOffices");
            DropForeignKey("dbo.ServiceForApproves", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.AccountForApproves", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ServiceForApproves", new[] { "Service_Id" });
            DropIndex("dbo.Vehicles", new[] { "Service_Id" });
            DropIndex("dbo.Vehicles", new[] { "VehicleType_Id" });
            DropIndex("dbo.Reservations", new[] { "Vehicle_Id" });
            DropIndex("dbo.Reservations", new[] { "ReturnBranchOffice_Id" });
            DropIndex("dbo.Reservations", new[] { "RentBranchOffice_Id" });
            DropIndex("dbo.Ratings", new[] { "Service_Id" });
            DropIndex("dbo.Comments", new[] { "Service_Id" });
            DropIndex("dbo.BranchOffices", new[] { "Service_Id" });
            DropIndex("dbo.AccountForApproves", new[] { "User_Id" });
            DropPrimaryKey("dbo.Services");
            DropPrimaryKey("dbo.VehicleTypes");
            DropPrimaryKey("dbo.Vehicles");
            DropPrimaryKey("dbo.Reservations");
            DropPrimaryKey("dbo.Ratings");
            DropPrimaryKey("dbo.Comments");
            DropPrimaryKey("dbo.BranchOffices");
            AlterColumn("dbo.Services", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.VehicleTypes", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Vehicles", "Service_Id", c => c.Int());
            AlterColumn("dbo.Vehicles", "VehicleType_Id", c => c.Int());
            AlterColumn("dbo.Vehicles", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Reservations", "Vehicle_Id", c => c.Int());
            AlterColumn("dbo.Reservations", "ReturnBranchOffice_Id", c => c.Int());
            AlterColumn("dbo.Reservations", "RentBranchOffice_Id", c => c.Int());
            AlterColumn("dbo.Reservations", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Ratings", "Service_Id", c => c.Int());
            AlterColumn("dbo.Ratings", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Comments", "Service_Id", c => c.Int());
            AlterColumn("dbo.Comments", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.BranchOffices", "Service_Id", c => c.Int());
            AlterColumn("dbo.BranchOffices", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.AspNetUsers", "IsApproved");
            DropColumn("dbo.AspNetUsers", "DocumentImage");
            DropTable("dbo.ServiceForApproves");
            DropTable("dbo.AccountForApproves");
            AddPrimaryKey("dbo.Services", "Id");
            AddPrimaryKey("dbo.VehicleTypes", "Id");
            AddPrimaryKey("dbo.Vehicles", "Id");
            AddPrimaryKey("dbo.Reservations", "Id");
            AddPrimaryKey("dbo.Ratings", "Id");
            AddPrimaryKey("dbo.Comments", "Id");
            AddPrimaryKey("dbo.BranchOffices", "Id");
            CreateIndex("dbo.Vehicles", "Service_Id");
            CreateIndex("dbo.Vehicles", "VehicleType_Id");
            CreateIndex("dbo.Reservations", "Vehicle_Id");
            CreateIndex("dbo.Reservations", "ReturnBranchOffice_Id");
            CreateIndex("dbo.Reservations", "RentBranchOffice_Id");
            CreateIndex("dbo.Ratings", "Service_Id");
            CreateIndex("dbo.Comments", "Service_Id");
            CreateIndex("dbo.BranchOffices", "Service_Id");
            AddForeignKey("dbo.Vehicles", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Ratings", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Comments", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.BranchOffices", "Service_Id", "dbo.Services", "Id");
            AddForeignKey("dbo.Vehicles", "VehicleType_Id", "dbo.VehicleTypes", "Id");
            AddForeignKey("dbo.Reservations", "Vehicle_Id", "dbo.Vehicles", "Id");
            AddForeignKey("dbo.Reservations", "ReturnBranchOffice_Id", "dbo.BranchOffices", "Id");
            AddForeignKey("dbo.Reservations", "RentBranchOffice_Id", "dbo.BranchOffices", "Id");
        }
    }
}
