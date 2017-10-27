namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeddate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Paystub", "StartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paystub", "StartDate");
        }
    }
}
