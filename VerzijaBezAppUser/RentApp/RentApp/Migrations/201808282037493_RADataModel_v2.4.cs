namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RADataModel_v24 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BanedManagers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BanedManagers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BanedManagers", new[] { "User_Id" });
            DropTable("dbo.BanedManagers");
        }
    }
}
