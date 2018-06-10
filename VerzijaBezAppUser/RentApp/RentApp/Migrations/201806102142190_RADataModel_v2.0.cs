namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RADataModel_v20 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "AppUserId", "dbo.AppUsers");
            DropIndex("dbo.AspNetUsers", new[] { "AppUserId" });
            AddColumn("dbo.Reservations", "UserId", c => c.String());
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "Image", c => c.String());
            AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "IsLogged", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Comments", "UserId", c => c.String());
            AlterColumn("dbo.Ratings", "UserId", c => c.String());
            AlterColumn("dbo.Services", "OwnerId", c => c.String());
            DropColumn("dbo.AspNetUsers", "AppUserId");
            DropTable("dbo.AppUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Image = c.String(),
                        DateOfBirth = c.DateTime(),
                        IsLogged = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "AppUserId", c => c.Int(nullable: false));
            AlterColumn("dbo.Services", "OwnerId", c => c.Int(nullable: false));
            AlterColumn("dbo.Ratings", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.Comments", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "IsLogged");
            DropColumn("dbo.AspNetUsers", "DateOfBirth");
            DropColumn("dbo.AspNetUsers", "Image");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.Reservations", "UserId");
            CreateIndex("dbo.AspNetUsers", "AppUserId");
            AddForeignKey("dbo.AspNetUsers", "AppUserId", "dbo.AppUsers", "Id", cascadeDelete: true);
        }
    }
}
