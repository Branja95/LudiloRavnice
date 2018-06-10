namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RADataModel_v11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AppUsers", "DateOfBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AppUsers", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
