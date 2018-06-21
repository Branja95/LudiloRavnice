namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RADataModel_v23 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccountForApproves", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AccountForApproves", new[] { "User_Id" });
            AddColumn("dbo.AccountForApproves", "UserId", c => c.String());
            DropColumn("dbo.AccountForApproves", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountForApproves", "User_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.AccountForApproves", "UserId");
            CreateIndex("dbo.AccountForApproves", "User_Id");
            AddForeignKey("dbo.AccountForApproves", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
